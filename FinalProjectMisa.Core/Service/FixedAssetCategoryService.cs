using FinalProjectMisa.Core.Dto;
using FinalProjectMisa.Core.Interface.Repository;
using FinalProjectMisa.Core.Interface.Service;

namespace FinalProjectMisa.Core.Service;

public class FixedAssetCategoryService : IFixedAssetCategoryService
{
    private readonly IFixedAssetCategoryRepo _fixedAssetCategoryRepo;
    public FixedAssetCategoryService(IFixedAssetCategoryRepo fixedAssetCategoryRepo)
    {
        _fixedAssetCategoryRepo = fixedAssetCategoryRepo;
    }
    public async Task<IEnumerable<FixedAssetCategoryDto>> GetFixedAssetCategoryAsync()
    {
        return await _fixedAssetCategoryRepo.GetFixedAssetCategories();
    }
}