using FluentResults;
using Labubu.Main.DAL;
using Labubu.Main.DAL.Enums;
using Labubu.Main.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Labubu.Main.Services
{
    public class ClothesService : IClothesService
    {
        private readonly MainContext _context;
        private readonly IMinioService _minioService;
        private readonly IDistributedCache _cache;
        private const int CacheExpirationMinutes = 60;

        public ClothesService(
            MainContext context, 
            IMinioService minioService,
            IDistributedCache cache)
        {
            _context = context;
            _minioService = minioService;
            _cache = cache;
        }

        public async Task<Result<List<Clothes>>> GetAllClothesAsync()
        {
            var clothes = await _context.Clothes.ToListAsync();
            return Result.Ok(clothes);
        }

        public async Task<Result<List<Clothes>>> GetClothesByTypeAsync(ClothesType type)
        {
            var clothes = await _context.Clothes
                .Where(c => c.Type == type)
                .ToListAsync();

            return Result.Ok(clothes);
        }

        public async Task<Result> SetClothesAsync(Guid userId, Guid clothesId)
        {
            var labubu = await _context.Labubus
                .Include(l => l.LabubuClothes)
                .FirstOrDefaultAsync(l => l.UserId == userId);

            if (labubu is null)
                return Result.Fail("Labubu не найден для этого пользователя");

            var clothes = await _context.Clothes.FindAsync(clothesId);
            if (clothes is null)
                return Result.Fail("Одежда не найдена");

            var existing = await _context.LabubuClothes
                .Include(lc => lc.Clothes)
                .Where(lc => lc.LabubuId == labubu.Id && lc.Clothes.Type == clothes.Type)
                .ToListAsync();

            if (existing.Any())
                _context.LabubuClothes.RemoveRange(existing);

            _context.LabubuClothes.Add(new LabubuClothes
            {
                LabubuId = labubu.Id,
                ClothesId = clothes.Id
            });

            await _context.SaveChangesAsync();
            await InvalidateClothesCacheAsync();
            
            return Result.Ok().WithSuccess($"Одежда '{clothes.Name}' успешно надета.");
        }

        private async Task InvalidateClothesCacheAsync()
        {
            try
            {
                var allTypes = Enum.GetValues<ClothesType>();
                var tasks = new List<Task>
                {
                    _cache.RemoveAsync("clothes_all")
                };
                
                foreach (var type in allTypes)
                {
                    tasks.Add(_cache.RemoveAsync($"clothes_{type}"));
                }
                
                await Task.WhenAll(tasks);
            }
            catch
            {
            }
        }

        public async Task<Result<List<ClothesDto>>> GetClothesAsync(ClothesType? bodyPartId)
        {
            var cacheKey = $"clothes_{bodyPartId?.ToString() ?? "all"}";
            
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                try
                {
                    var cachedResult = JsonSerializer.Deserialize<List<ClothesDto>>(cachedData);
                    if (cachedResult != null)
                    {
                        return Result.Ok(cachedResult);
                    }
                }
                catch
                {
                }
            }

            var query = _context.Clothes.AsQueryable();

            if (bodyPartId.HasValue)
            {
                query = query.Where(c => c.Type == bodyPartId.Value);
            }

            var clothes = await query.ToListAsync();

            var urlTasks = clothes.Select(async cloth =>
            {
                var urlResult = await _minioService.GetFileUrlAsync(cloth.Url);
                return new ClothesDto(cloth.Id, cloth.Name, urlResult.IsSuccess ? urlResult.Value : cloth.Url);
            });

            var result = await Task.WhenAll(urlTasks);
            var clothesList = result.ToList();

            try
            {
                var jsonData = JsonSerializer.Serialize(clothesList);
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheExpirationMinutes)
                };
                await _cache.SetStringAsync(cacheKey, jsonData, cacheOptions);
            }
            catch
            {
            }

            return Result.Ok(clothesList);
        }
    }
}
