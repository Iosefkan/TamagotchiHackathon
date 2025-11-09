using Labubu.Main.Data;
using Microsoft.EntityFrameworkCore;

namespace Labubu.Main.Services;

public class MessageBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MessageBackgroundService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(6);
    private readonly TimeSpan _minMessageInterval = TimeSpan.FromHours(12);

    public MessageBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<MessageBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessMessagesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in message background service");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private async Task ProcessMessagesAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MainContext>();
        var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();

        var now = DateTime.UtcNow;
        var users = await context.Users
            .Include(u => u.Labubu)
            .Where(u => u.Labubu != null 
                && u.Labubu.IsMessageRead 
                && (u.Labubu.LastMessageGeneratedAt == null 
                    || now - u.Labubu.LastMessageGeneratedAt.Value >= _minMessageInterval))
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Processing messages for {Count} users", users.Count);

        foreach (var user in users)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            try
            {
                await messageService.GenerateAndSendMessageAsync(user.Id);
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message for user {UserId}", user.Id);
            }
        }
    }
}

