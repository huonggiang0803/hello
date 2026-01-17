using FinalProjectMisa.Core.Dto;

namespace FinalProjectMisa.Core.Interface.Service;

public interface IDepartmentService
{
    Task<IEnumerable<DepartmentDto>> GetDepartmentsAsync();

}