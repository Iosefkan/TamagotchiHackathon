using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Common.Migrations;

public static class MigrationApplyService
{
    public static async Task ApplyMigrations(this DatabaseFacade database, string assemblyName,
        CancellationToken token = default)
    {
        var migrator = database.GetService<IMigrator>();
        var pendingMigrations = await database.GetPendingMigrationsAsync(token);

        if (pendingMigrations.Any())
        {
            await migrator.MigrateAsync(null, token);
        }
    }
}