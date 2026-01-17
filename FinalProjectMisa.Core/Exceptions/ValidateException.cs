namespace FinalProjectMisa.Core.Exceptions;

public class ValidateException : Exception
{
    public string UserMsg { get; set; }
    public object? Errors { get; set; }

    // Trường hợp 1: Bắn lỗi nghiệp vụ đơn lẻ (Ví dụ: Tỷ lệ hao mòn sai)
    public ValidateException(string userMsg) : base(userMsg)
    {
        UserMsg = userMsg;
    }

    // Trường hợp 2: Bắn lỗi validate nhiều trường (Ví dụ: Thiếu Mã, Thiếu Tên cùng lúc)
    public ValidateException(string userMsg, object errors) : base(userMsg)
    {
        UserMsg = userMsg;
        Errors = errors;
    }
}