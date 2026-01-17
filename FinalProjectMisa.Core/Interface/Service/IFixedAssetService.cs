using FinalProjectMisa.Core.Dto;
using FinalProjectMisa.Core.Entities;

namespace FinalProjectMisa.Core.Interface.Service;

public interface IFixedAssetService : IBaseService<FixedAssetEntity>
{
    Task<IEnumerable<FixedAssetDto>> GetAllWithDetailsAsync();
    Task<FixedAssetDto> GetAssetByIdWithDetailAsync(Guid assetId);
    Task<PagedResult<FixedAssetDto>> GetFilterAsync(FixedAssetFilterDto filterDto);
    Task<int> DeleteManyAsync(List<Guid> assetIds);
    Task<string> GetNewCodeAsync();
    public Task<FixedAssetDto> ReplicateAssetAsync(Guid id);
}