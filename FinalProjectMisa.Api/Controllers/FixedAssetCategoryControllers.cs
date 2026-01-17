using FinalProjectMisa.Core.Interface.Service;
using Microsoft.AspNetCore.Mvc;

namespace FinalProjectMisa.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class FixedAssetCategoryControllers : ControllerBase
{
    private readonly IFixedAssetCategoryService _fixedAssetCategoryService;

    public FixedAssetCategoryControllers(IFixedAssetCategoryService fixedAssetCategoryService)
    {
        _fixedAssetCategoryService = fixedAssetCategoryService;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _fixedAssetCategoryService.GetFixedAssetCategoryAsync();
        return Ok(data);
    }
}