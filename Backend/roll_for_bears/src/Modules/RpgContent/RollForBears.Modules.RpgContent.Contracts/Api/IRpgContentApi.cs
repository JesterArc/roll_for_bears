using RollForBears.Modules.RpgContent.Contracts.DTOs;

namespace RollForBears.Modules.RpgContent.Contracts.Api;

public interface IRpgContentApi
{
    public Task<ICollection<GameSystemDto>> GetGameSystemAsync(CancellationToken cancellationToken);
    
    public Task<GameSystemDto> CreateGameSystemAsync(CreateGameSystemRequestDto request, CancellationToken cancellationToken);
}