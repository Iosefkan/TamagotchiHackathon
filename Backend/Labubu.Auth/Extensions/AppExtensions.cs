using Hangfire;
using Microsoft.Extensions.Options;
using PetApp.AuthService.Data.Models.Options;
using PetApp.AuthService.Jobs;

namespace Labubu.Auth.Extensions;

public static class AppExtensions
{
    public static void ConfigureTokenCleanupJob(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices
            .GetRequiredService<IOptions<TokenCleanupOptions>>()
            .Value;
        
        var recurringJobManager = app.ApplicationServices
            .GetRequiredService<IRecurringJobManager>();
        
        recurringJobManager.AddOrUpdate<TokenCleanupJob>(
            options.JobId,
            job => job.CleanupExpiredAndRevokedTokensAsync(),
            options.CronExpression);
    }
}