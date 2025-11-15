using Common;
using Common.Migrations;
using FluentValidation;
using Labubu.Auth.Configuration;
using Labubu.Auth.Data;
using Labubu.Auth.Endpoints;
using Labubu.Auth.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IO;
using System.Text;
using Common.Extensions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.StackExchangeRedis;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (connectionString != null)
{
    builder.Services.AddDbContext<AuthDbContext>(options =>
        options.UseNpgsql(connectionString));
}

builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtConfig = builder.Configuration.GetSection("Jwt").Get<JwtConfig>()!;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtConfig.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtConfig.Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddOptions<RabbitMqTransportOptions>()
    .Configure(options => builder.Configuration.GetSection("Rabbit").Bind(options));

builder.Services.AddMassTransit(massTransitConfig =>
{
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
});

var redisBlacklistConnection = builder.Configuration.GetConnectionString("RedisBlacklist");
if (!string.IsNullOrEmpty(redisBlacklistConnection))
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisBlacklistConnection;
    });
}

builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();

builder.AddValidation();

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new System.IO.DirectoryInfo("/app/data-protection-keys"))
    .SetApplicationName("LabubuAuth");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName!.Replace("+", "."));
    options.AddSecurity();
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddHostedService<Migrator<AuthDbContext>>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

Signup.Map(app);
Signin.Map(app);
RefreshToken.Map(app);
Logout.Map(app);

app.Run();