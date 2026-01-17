using FinalProjectMisa.Core.Dto;

namespace FinalProjectMisa.Core.Interface.Repository;

public interface IDepartmentRepo
{
    Task<IEnumerable<DepartmentDto>> GetDepartment();
}