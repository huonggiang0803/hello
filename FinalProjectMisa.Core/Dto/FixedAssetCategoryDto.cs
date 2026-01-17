namespace FinalProjectMisa.Core.Dto;

public class FixedAssetCategoryDto
{
    public Guid fixed_asset_category_id { get; set; }
    public string fixed_asset_category_code { get; set; }
    public string fixed_asset_category_name { get; set; }
    public int? life_time { get; set; }
    public decimal? depreciation_rate { get; set; }
    
}