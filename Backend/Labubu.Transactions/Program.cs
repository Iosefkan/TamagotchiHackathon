using Serilog;
using TickerQ.DependencyInjection;
using Transactions.Extensions;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateSlimBuilder(args);

    builder
        .ConfigureOptions()
        .ConfigureRabbitMq()
        .ConfigurePostgre()
        //.ConfigureScheduler()
        .ConfigureCache()
        .ConfigureServices();

    var app = builder.Build();
    //app.UseTickerQ();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}