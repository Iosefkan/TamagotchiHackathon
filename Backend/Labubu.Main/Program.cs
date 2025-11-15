using Common;
using Common.Extensions;
using Common.Migrations;
using FluentValidation;
using Labubu.Main.Configuration;
using Labubu.Main.Data;
using Labubu.Main.Endpoints;
using Labubu.Main.Extensions;
using Labubu.Main.Hubs;
using Labubu.Main.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using Shared.Contracts;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (connectionString != null)
{
    builder.Services.AddDbContext<MainContext>(options =>
        options.UseNpgsql(connectionString));
}

var redisCacheConnection = builder.Configuration.GetConnectionString("RedisCache");
if (!string.IsNullOrEmpty(redisCacheConnection))
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisCacheConnection;
    });
}

builder.Services.Configure<Common.Models.JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddJwt(withAuth: true);

builder.Services.Configure<MinioConfig>(builder.Configuration.GetSection("Minio"));
builder.Services.AddScoped<IMinioService, MinioService>();

builder.Services.Configure<OpenRouterConfig>(builder.Configuration.GetSection("OpenRouter"));
builder.Services.AddHttpClient();

builder.Services.AddOptions<RabbitMqTransportOptions>()
    .Configure(options => builder.Configuration.GetSection("Rabbit").Bind(options));

builder.Services.AddMassTransit(massTransitConfig =>
{
    massTransitConfig.AddConsumers(typeof(Program).Assembly);
    
    massTransitConfig.UsingRabbitMq((context, rabbitConfig) =>
    {
        var rabbitOptions = context.GetRequiredService<IOptions<RabbitMqTransportOptions>>().Value;
        rabbitConfig.Host(rabbitOptions.Host, rabbitOptions.Port, rabbitOptions.VHost, h =>
        {
            h.Username(rabbitOptions.User);
            h.Password(rabbitOptions.Pass);
        });
        
        rabbitConfig.ConfigureEndpoints(context);
    });
    
    massTransitConfig.AddRequestClient<BankInfoRequest>(TimeSpan.FromSeconds(30));
    massTransitConfig.AddRequestClient<UserStatisticsRequest>(TimeSpan.FromSeconds(60));
});

builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<BankService>();
builder.Services.AddScoped<IClothesService, ClothesService>();
builder.Services.AddScoped<ILabubuService, LabubuService>();
builder.Services.AddScoped<IAchievementService, AchievementService>();
builder.Services.AddScoped<IBattlePassService, BattlePassService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMessageGenerationService, MessageGenerationService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IFinanceService, FinanceService>();

builder.Services.AddHostedService<MessageBackgroundService>();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName!.Replace("+", "."));
    options.AddSecurity();
});

builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddHostedService<Migrator<MainContext>>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

GetHealth.Map(app);
GetCurrentClothes.Map(app);
LabubuAction.Map(app);
GetClothes.Map(app);
SetClothe.Map(app);
GetAchievements.Map(app);
GameStart.Map(app);
GameEnd.Map(app);
GetUserEnergy.Map(app);
GetUserInfo.Map(app);
MarkMessageRead.Map(app);
GetFinanceSummary.Map(app);

GetActiveBattlePasses.Map(app);
GetBattlePassInfo.Map(app);
GetBattlePassProgress.Map(app);
GetBattlePassRewards.Map(app);
PurchaseBattlePass.Map(app);
ClaimBattlePassReward.Map(app);

app.MapHub<MessageHub>(ApiRoutes.Hubs.Message);

app.Run();