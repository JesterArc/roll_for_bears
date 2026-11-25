using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RollForBears.Modules.RpgContent.Contracts.Api;
using RollForBears.Modules.RpgContent.Contracts.DTOs;
using RollForBears.Modules.RpgContent.Database;
using RollForBears.Modules.RpgContent.Models;

namespace RollForBears.Modules.RpgContent.Services;

public sealed class RpgContentService : IRpgContentApi
{
    private readonly RpgContentDbContext _dbContext;
    
    public RpgContentService(RpgContentDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<ICollection<GameSystemDto>> GetGameSystemAsync(CancellationToken cancellationToken)
    {
        var gameSystems = await _dbContext.GameSystems
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return gameSystems.ConvertAll(ConvertToGameSystemDto);
    }

    public async Task<GameSystemDto> CreateGameSystemAsync(CreateGameSystemRequestDto request, CancellationToken cancellationToken)
    {
        
        var gameSystem = new GameSystem()
        {
            GameSystemUuid = Guid.NewGuid(),
            SystemKey = request.SystemKey,
            Name = request.Name,
            GenreSet = request.GenreSet,
            Description = request.Description,
            Settings = request.Settings ?? JsonSerializer.SerializeToElement(new { })
        };
        
        _dbContext.GameSystems.Add(gameSystem);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return ConvertToGameSystemDto(gameSystem);
    }

    public static GameSystemDto ConvertToGameSystemDto(GameSystem gameSystem)
    {
        GameSystemDto dto = new GameSystemDto()
        {
            GameSystemUuid = gameSystem.GameSystemUuid,
            SystemKey = gameSystem.SystemKey,
            Name = gameSystem.Name,
            GenreSet = gameSystem.GenreSet,
            Description = gameSystem.Description,
            Settings = gameSystem.Settings
        };
        
        return dto;
    }
}