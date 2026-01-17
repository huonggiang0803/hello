namespace FinalProjectMisa.Core.Dto;

public class FixedAssetFilterDto
{
    public string? Keyword { get; set; }         // Tìm kiếm chung
    public Guid? DepartmentId { get; set; }      // Lọc theo phòng ban
    public Guid? FixedAssetCategoryId { get; set; } // Lọc theo loại
    public int PageNumber { get; set; } = 1;     // Trang hiện tại (Mặc định 1)
    public int PageSize { get; set; } = 20; // Số bản ghi/trang (Mặc định 20)
}