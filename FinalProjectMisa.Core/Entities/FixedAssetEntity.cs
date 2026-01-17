using FinalProjectMisa.Core.MisaAttribute;

namespace FinalProjectMisa.Core.Entities;

[MISATableName("fixed_asset")]
public class FixedAssetEntity : BaseEntity
{
    /// <summary>
    /// Khóa chính của tài sản
    /// </summary>
    
    [MISAPrimaryKey]
    public Guid fixed_asset_id { get; set; }

    /// <summary>
    /// Mã tài sản
    /// </summary>
    [MISARequired("Mã tài sản")]
    [MISAMaxLength(20, "Mã tài sản không quá 20 ký tự")]
    public string fixed_asset_code { get; set; }

    /// <summary>
    /// Tên tài sản 
    /// </summary>
    [MISARequired("Tên tài sản")]
    public string fixed_asset_name { get; set; }

    /// <summary>
    /// Khóa ngoại liên kết với bảng bộ phận (Department)
    /// </summary>
    // [MISARequired("Mã bộ phận sử dụng")]
    public Guid department_id { get; set; }

    /// <summary>
    /// Khóa ngoại liên kết với bảng loại tài sản (FixedAssetCategory)
    /// </summary>
    [MISARequired("Mã loại tài sản")]
    public Guid fixed_asset_category_id { get; set; }

    /// <summary>
    /// Ngày mua tài sản
    /// </summary>
    [MISARequired("Ngày mua")]
    public DateTime purchase_date { get; set; }

    /// <summary>
    /// Số năm sử dụng của tài sản
    /// </summary>
    [MISARequired("Số năm sử dụng")]
    public int? usage_years { get; set; }

    /// <summary>
    /// Tỷ lệ hao mòn (%)
    /// </summary>
    [MISARequired("Tỉ lệ hao mòn")]
    public decimal depreciation_rate { get; set; }

    /// <summary>
    /// Năm bắt đầu sử dụng 
    /// </summary>
    [MISARequired("Năm sử dụng")]
    public int? production_year { get; set; }

    /// <summary>
    /// Năm bắt đầu theo dõi tài sản trên phần mềm
    /// </summary>
    public int? tracked_year { get; set; }

    /// <summary>
    /// Số lượng tài sản
    /// </summary>
    [MISARequired("Số lượng tài sản")]
    public decimal quantity { get; set; }

    /// <summary>
    /// Nguyên giá tài sản
    /// </summary>
    [MISARequired("Nguyên giá tài sản")]
    public decimal cost { get; set; }

    /// <summary>
    /// Giá trị hao mòn trong một năm (Tính theo tỷ lệ hao mòn * Nguyên giá)
    /// </summary>
    [MISARequired("Giá trị hao mòn trong một năm")]
    public decimal depreciation_value { get; set; }
    // public string department_name { get; set; }
    // public string fixed_asset_category_name { get; set; }
}