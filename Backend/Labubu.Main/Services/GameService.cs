using FluentResults;
using Labubu.Main.DAL;
using Labubu.Main.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Labubu.Main.Services;

public class GameService : IGameService
{
    private readonly MainContext _context;
    private readonly IBattlePassService _battlePassService;
    private readonly IDistributedCache _cache;

    public GameService(
        MainContext context,
        IBattlePassService battlePassService,
        IDistributedCache cache)
    {
        _context = context;
        _battlePassService = battlePassService;
        _cache = cache;
    }

    public async Task<Result> StartGameAsync(Guid labubuId, int gameId)
    {
        var labubu = await _context.Labubus.FindAsync(labubuId);
        if (labubu == null)
            return Result.Fail("Labubu not found");

        var gameGuid = GetGameGuidFromInt(gameId);
        var game = await _context.Games.FindAsync(gameGuid);
        if (game == null)
        {
            game = new Game
            {
                Id = gameGuid,
                Name = $"Game {gameId}",
                Description = $"Game with ID {gameId}"
            };
            _context.Games.Add(game);
            await _context.SaveChangesAsync();
        }

        var session = new GameSession
        {
            Id = Guid.NewGuid(),
            GameId = gameGuid,
            LabubuId = labubuId,
            StartedAt = DateTime.UtcNow
        };

        _context.GameSessions.Add(session);
        await _context.SaveChangesAsync();

        return Result.Ok();
    }

    public async Task<Result<GameEndDto>> EndGameAsync(Guid labubuId, int gameId, int score, JsonDocument? specificData)
    {
        var gameGuid = GetGameGuidFromInt(gameId);
        var session = await _context.GameSessions
            .Where(gs => gs.LabubuId == labubuId && gs.GameId == gameGuid && gs.EndedAt == null)
            .OrderByDescending(gs => gs.StartedAt)
            .FirstOrDefaultAsync();

        if (session == null)
            return Result.Fail("Active game session not found");

        session.Score = score;
        session.SpecificData = specificData;
        session.EndedAt = DateTime.UtcNow;

        int? award = score > 0 ? score / 10 : null;
        session.Award = award;

        var labubu = await _context.Labubus.FindAsync(labubuId);
        if (labubu != null && award.HasValue)
        {
            labubu.Energy = Math.Min(100, labubu.Energy + award.Value);
            labubu.Health = Math.Min(100, labubu.Health + award.Value / 2);
        }

        await _context.SaveChangesAsync();

        if (labubu != null)
        {
            try
            {
                await _cache.RemoveAsync($"user_energy_{labubu.UserId}");
            }
            catch
            {
            }
        }

        var battlePassXp = Math.Max(10, (score / 100) * 10);
        await _battlePassService.AddXpAsync(labubuId, battlePassXp, $"GameEnd_{gameId}");

        return Result.Ok(new GameEndDto(award));
    }

    private static Guid GetGameGuidFromInt(int gameId)
    {
        var bytes = new byte[16];
        BitConverter.GetBytes(gameId).CopyTo(bytes, 0);
        return new Guid(bytes);
    }
}
