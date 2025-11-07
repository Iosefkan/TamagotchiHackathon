using Microsoft.EntityFrameworkCore;
using Transactions.DAL;

namespace Transactions.Services;

public class MigrationService(IServiceProvider serviceProvider, ILogger<MigrationService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            
            var dbContext = scope.ServiceProvider.GetRequiredService<TransactionsDataContext>();

            await dbContext.Database.MigrateAsync(cancellationToken);
        }
        catch (Exception exc)
        {
            logger.LogError(exc, "Failed to apply migrations");
            Environment.FailFast("Failed to apply migrations", exc);
        }
    }
}