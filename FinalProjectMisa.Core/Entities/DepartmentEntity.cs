namespace FinalProjectMisa.Core.Entities;

public class DepartmentEntity : BaseEntity
{
    /// <summary>
    /// Khóa chính
    /// </summary>
    public Guid department_id { get; set; } 
    /// <summary>
    /// Mã bộ phận
    /// </summary>
    public string department_code { get; set; }
    /// <summary>
    /// Tên bộ phận
    /// </summary>
    public string department_name { get; set; }
}