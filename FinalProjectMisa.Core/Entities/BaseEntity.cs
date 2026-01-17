namespace FinalProjectMisa.Core.Entities;

public abstract class BaseEntity
{
    
    /// <summary>
    /// Ngày tạo bản ghi
    /// </summary>
    public DateTime? created_date { get; set; }

    /// <summary>
    /// Người tạo bản ghi
    /// </summary>
    public string? created_by { get; set; }

    /// <summary>
    /// Ngày sửa bản ghi gần nhất
    /// </summary>
    public DateTime? modified_date { get; set; }

    /// <summary>
    /// Người sửa bản ghi gần nhất
    /// </summary>
    public string? modified_by { get; set; }
}