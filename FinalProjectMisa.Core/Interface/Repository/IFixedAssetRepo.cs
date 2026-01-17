using FinalProjectMisa.Core.Dto;
using FinalProjectMisa.Core.Entities;

namespace FinalProjectMisa.Core.Interface.Repository;

public interface IFixedAssetRepo : IBaseRepo<FixedAssetEntity>
{
    Task<IEnumerable<FixedAssetDto>> GetAllWithDetailsAsync();
    Task<PagedResult<FixedAssetDto>> GetFilterAsync(FixedAssetFilterDto filterDto);  
    public Task<FixedAssetDto> GetAssetByIdWithDetailAsync(Guid assetId);
    public Task<FixedAssetDto> ReplicateAssetAsync(Guid oldId, string newCode);
}