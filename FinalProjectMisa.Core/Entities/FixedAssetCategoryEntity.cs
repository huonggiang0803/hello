namespace FinalProjectMisa.Core.Entities;

public class FixedAssetCategoryEntity : BaseEntity
{
 // Bảng loại sản cố định
 
 /// <summary>
 /// Khóa chính
 /// </summary>
 public Guid fixed_asset_category_id { get; set; }
 /// <summary>
 /// Mã loại tài sản
 /// </summary>
 public string fixed_asset_category_code { get; set; }
 /// <summary>
 /// Tên loại tài sản
 /// </summary>
 public string fixed_asset_category_name { get; set; }
 /// <summary>
 /// Số năm sử dụng
 /// </summary>
 public int? life_time { get; set; }
 /// <summary>
 /// Tỉ lệ hao mòn (%)
 /// </summary>
 public decimal? depreciation_rate { get; set; }
}