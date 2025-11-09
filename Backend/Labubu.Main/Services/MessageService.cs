using FluentResults;
using Labubu.Main.Data;
using Labubu.Main.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;

namespace Labubu.Main.Services;

public class MessageService : IMessageService
{
    private readonly MainContext _context;
    private readonly IHubContext<MessageHub> _hubContext;
    private readonly ITransactionService _transactionService;
    private readonly IMessageGenerationService _messageGenerationService;

    public MessageService(
        MainContext context,
        IHubContext<MessageHub> hubContext,
        ITransactionService transactionService,
        IMessageGenerationService messageGenerationService)
    {
        _context = context;
        _hubContext = hubContext;
        _transactionService = transactionService;
        _messageGenerationService = messageGenerationService;
    }

    public async Task<Result> MarkMessageAsReadAsync(Guid labubuId)
    {
        var labubu = await _context.Labubus.FindAsync(labubuId);
        if (labubu == null)
            return Result.Fail("Labubu not found");

        labubu.IsMessageRead = true;
        await _context.SaveChangesAsync();

        return Result.Ok();
    }

    public async Task<Result> GenerateAndSendMessageAsync(Guid userId)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.Labubu)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user?.Labubu == null)
                return Result.Fail("User or Labubu not found");

            var labubu = user.Labubu;

            if (!labubu.IsMessageRead)
            {
                return Result.Ok().WithSuccess("Previous message not read yet");
            }

            var toDate = DateTimeOffset.UtcNow;
            var fromDate = toDate.AddDays(-30);
            
            var statisticsResult = await _transactionService.GetUserStatisticsAsync(
                user.Name,
                fromDate,
                toDate);

            if (statisticsResult.IsFailed)
            {
                var defaultMessage = $"Сегодня {DateTime.UtcNow:dd MMM}: Пока нет данных о транзакциях, но ваш Labubu готов к приключениям! 🎮";
                labubu.Message = defaultMessage;
                labubu.IsMessageRead = false;
                labubu.LastMessageGeneratedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                
                await _hubContext.Clients.Group($"user_{userId}").SendAsync("ReceiveMessage", new
                {
                    message = defaultMessage,
                    timestamp = DateTime.UtcNow
                });
                
                return Result.Ok().WithSuccess("Default message sent");
            }

            var statistics = statisticsResult.Value;

            var today = DateTime.UtcNow;
            var timezone = "Europe/Moscow";
            var currency = "€";
            var messageResult = await _messageGenerationService.GenerateMessageAsync(
                statistics,
                today,
                timezone,
                currency);

            if (messageResult.IsFailed)
            {
                return Result.Fail("Failed to generate message");
            }

            var message = messageResult.Value;

            labubu.Message = message;
            labubu.IsMessageRead = false;
            labubu.LastMessageGeneratedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            await _hubContext.Clients.Group($"user_{userId}").SendAsync("ReceiveMessage", new
            {
                message = message,
                timestamp = DateTime.UtcNow
            });

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error: {ex.Message}");
        }
    }
}
