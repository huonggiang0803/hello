using FinalProjectMisa.Core.Entities;

namespace FinalProjectMisa.Core.Dto;

public class FixedAssetDto
{
    /// <summary>
    /// Khóa chính của tài sản
    /// </summary>
    public Guid fixed_asset_id { get; set; }

    /// <summary>
    /// Mã tài sản
    /// </summary>
    public string fixed_asset_code { get; set; }

    /// <summary>
    /// Tên tài sản
    /// </summary>
    public string fixed_asset_name { get; set; }

    /// <summary>
    /// Nguyên giá
    /// </summary>
    public decimal cost { get; set; }
    /// <summary>
    /// Số năm sử dụng của tài sản
    /// </summary>
    public int? usage_years { get; set; }
    /// <summary>
    /// Tỷ lệ hao mòn (%)
    /// </summary>
    public decimal depreciation_rate { get; set; }
    /// <summary>
    /// Năm bắt đầu sử dụng 
    /// </summary>
    public int? production_year { get; set; }
// <summary>
    /// Năm bắt đầu theo dõi tài sản trên phần mềm
    /// </summary>
    public int? tracked_year { get; set; }
    
    /// <summary>
    /// Số lượng
    /// </summary>
    public decimal quantity { get; set; }
    /// <summary>
    /// Ngày mua tài sản
    /// </summary>
    public DateTime purchase_date { get; set; }
    /// <summary>
    /// Giá trị hao mòn năm
    /// </summary>
    /// <summary>
    /// Khóa chính
    /// </summary>
    public Guid department_id { get; set; }

    /// <summary>
    /// Mã bộ phận
    /// </summary>
    public string department_code { get; set; }

    public decimal depreciation_value { get; set; }

    /// <summary>
    /// Tên bộ phận sử dụng (Lấy từ bảng department)
    /// </summary>
    public string department_name { get; set; }

    /// <summary>
    /// Khóa chính
    /// </summary>
    public Guid fixed_asset_category_id { get; set; }

    /// <summary>
    /// Mã loại tài sản
    /// </summary>
    public string fixed_asset_category_code { get; set; }

    /// <summary>
    /// Tên loại tài sản (Lấy từ bảng fixed_asset_category)
    /// </summary>
    public string fixed_asset_category_name { get; set; }
}