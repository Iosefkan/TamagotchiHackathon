using FluentResults;

namespace Labubu.Main.Services;

public interface IMessageService
{
    Task<Result> MarkMessageAsReadAsync(Guid labubuId);
    Task<Result> GenerateAndSendMessageAsync(Guid userId);
}

