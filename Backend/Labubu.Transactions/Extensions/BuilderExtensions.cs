using MassTransit;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TickerQ.Dashboard.DependencyInjection;
using TickerQ.DependencyInjection;
using Transactions.Configuration;
using Transactions.DAL;
using Transactions.Services;
using Transactions.Services.Banking;

namespace Transactions.Extensions;

public static class BuilderExtensions
{
    public static IHostApplicationBuilder ConfigureOptions(this IHostApplicationBuilder builder)
    {
        var config = builder.Configuration;
        builder.Services.Configure<AppConfig>(config.GetSection("App"));
        builder.Services.Configure<BanksConfig>(config.GetSection("Bank"));
        return builder;
    }

    public static IHostApplicationBuilder ConfigureRabbitMq(this IHostApplicationBuilder builder)
    {
        var config = builder.Configuration;
        builder.Services.AddOptions<RabbitMqTransportOptions>()
            .Configure(options => config.GetSection("Rabbit").Bind(options));

        builder.Services.AddMassTransit(massTransitConfig =>
        {
            massTransitConfig.UsingRabbitMq((context, rabbitConfig) =>
            {
                rabbitConfig.ConfigureEndpoints(context);
            });
        });
        return builder;
    }

    public static IHostApplicationBuilder ConfigurePostgre(this IHostApplicationBuilder builder)
    {
        var config = builder.Configuration;
        var connectionString = config.GetConnectionString("DefaultConnection");
        if (connectionString is null) throw new ApplicationException("Connection string is null");
        builder.Services.AddDbContextPool<TransactionsDataContext>(options => options.UseNpgsql(connectionString));
        return builder;
    }
    
    public static IHostApplicationBuilder ConfigureScheduler(this IHostApplicationBuilder builder)
    {
        builder.Services.AddTickerQ(options =>
        {
            options.AddDashboard(dashboardOptions =>
            {
                dashboardOptions.SetBasePath("/tickerq-dashboard");
                dashboardOptions.WithApiKey("tickerq-api-key");
            });
        });
        return builder;
    }

    public static IHostApplicationBuilder ConfigureCache(this IHostApplicationBuilder builder)
    {
        var config = builder.Configuration;
        builder.Services.AddDistributedRedisCache(options =>
        {
            options.Configuration = config.GetConnectionString("RedisConnection");
            options.InstanceName = "master";
        });
        return builder;
    }
    
    public static IHostApplicationBuilder ConfigureServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSerilog();
        builder.Services.AddScoped<IBankingService, BankingService>();
        builder.Services.AddHostedService<MigrationService>();
        builder.Services.AddMemoryCache();
        return builder;
    }
    
}