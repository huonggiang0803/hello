using FinalProjectMisa.Core.Dto;
using FinalProjectMisa.Core.Interface.Repository;
using FinalProjectMisa.Core.Interface.Service;

namespace FinalProjectMisa.Core.Service;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepo _departmentRepo;

    public DepartmentService(IDepartmentRepo departmentRepo)
    {
        _departmentRepo = departmentRepo;
    }
    

    public async Task<IEnumerable<DepartmentDto>> GetDepartmentsAsync()
    {
        return await _departmentRepo.GetDepartment();
    }
}