using FinalProjectMisa.Core.Interface.Service;
using Microsoft.AspNetCore.Mvc;

namespace FinalProjectMisa.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class DepartmentControllers : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentControllers(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var departments = await _departmentService.GetDepartmentsAsync();
        return Ok(departments);
    }
}