using FinalProjectMisa.Core.Dto;
using FinalProjectMisa.Core.Entities;

namespace FinalProjectMisa.Core.Interface.Repository;

public interface IFixedAssetCategoryRepo
{
    public Task<IEnumerable<FixedAssetCategoryDto>> GetFixedAssetCategories();
}