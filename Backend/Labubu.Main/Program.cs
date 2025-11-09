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
using StackExchange.Redis;
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

var redisBlacklistConnection = builder.Configuration.GetConnectionString("RedisBlacklist");
if (!string.IsNullOrEmpty(redisBlacklistConnection))
{
    builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
        ConnectionMultiplexer.Connect(redisBlacklistConnection));
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

builder.Services.AddHostedService<MessageBackgroundService>();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Labubu Main API",
        Version = "v1",
        Description = "API for Labubu Main Service",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Labubu Team"
        }
    });
    
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });
    
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "swagger/{documentName}/swagger.json";
    });
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Labubu Main API v1");
        c.RoutePrefix = "swagger";
    });
}

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

GetActiveBattlePasses.Map(app);
GetBattlePassInfo.Map(app);
GetBattlePassProgress.Map(app);
GetBattlePassRewards.Map(app);
PurchaseBattlePass.Map(app);
ClaimBattlePassReward.Map(app);

app.MapHub<MessageHub>(ApiRoutes.Hubs.Message);

app.Run();