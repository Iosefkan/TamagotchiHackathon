using FluentResults;
using System.Text.Json;

namespace Labubu.Main.Services;

public interface IGameService
{
    Task<Result> StartGameAsync(Guid labubuId, int gameId);
    Task<Result<GameEndDto>> EndGameAsync(Guid labubuId, int gameId, int score, JsonDocument? specificData);
}

public record GameEndDto(int? Award);

