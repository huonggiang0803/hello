namespace FinalProjectMisa.Core.Dto;

public class ErrorResponse
{
    /// <summary>
    /// Thông báo lỗi dành cho User
    /// </summary>
    public string UserMsg { get; set; }

    /// <summary>
    /// Mã lỗi nội bộ (Ví dụ: 001, 002, 400, 500...)
    /// </summary>
    public string ErrorCode { get; set; }
    /// <summary>
    /// Mã tra cứu log (TraceId)
    /// </summary>
    public string TraceId { get; set; }

    /// <summary>
    /// Danh sách lỗi chi tiết (dùng khi validate form nhiều trường)
    /// </summary>
    public object? Errors { get; set; }
}