using Dapper;
using FinalProjectMisa.Core.Dto;
using FinalProjectMisa.Core.Entities;
using FinalProjectMisa.Core.Interface.Repository;
using Microsoft.Extensions.Configuration;

namespace FinalProjectMisa.Infrastructure.Repository;

public class FixedAssetCategoryRepo :  BaseRepo<FixedAssetCategoryEntity>, IFixedAssetCategoryRepo
{
    public FixedAssetCategoryRepo(IConfiguration configuration) : base(configuration)
    {
    }
    public async Task<IEnumerable<FixedAssetCategoryDto>> GetFixedAssetCategories()
    {
        using var _connection = CreateConnection();

        // Không cần AS, Dapper tự map
        var sql = $@"
        SELECT 
            {nameof(FixedAssetCategoryEntity.fixed_asset_category_id)},
            {nameof(FixedAssetCategoryEntity.fixed_asset_category_code)},
            {nameof(FixedAssetCategoryEntity.fixed_asset_category_name)},
                    {nameof(FixedAssetCategoryEntity.life_time)},
                    {nameof(FixedAssetCategoryEntity.depreciation_rate)}

        FROM fixed_asset_category
        ORDER BY {nameof(FixedAssetCategoryEntity.fixed_asset_category_code)}";

        return await _connection.QueryAsync<FixedAssetCategoryDto>(sql);
    }
}