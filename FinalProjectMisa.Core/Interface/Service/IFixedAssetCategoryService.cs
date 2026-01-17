using FinalProjectMisa.Core.Dto;

namespace FinalProjectMisa.Core.Interface.Service;

public interface IFixedAssetCategoryService
{
    Task<IEnumerable<FixedAssetCategoryDto>> GetFixedAssetCategoryAsync();
}