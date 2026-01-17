using FinalProjectMisa.Core.Dto;
using FinalProjectMisa.Core.Entities;
using FinalProjectMisa.Core.Interface.Service;
using Microsoft.AspNetCore.Mvc;

namespace FinalProjectMisa.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class FixedAssetsController : ControllerBase
{
    private readonly IFixedAssetService _fixedAssetService;

    public FixedAssetsController(IFixedAssetService fixedAssetService)
    {
        _fixedAssetService = fixedAssetService;
    }
    // [HttpGet]
    // public async Task<IActionResult> GetAll()
    // {
    //     var data = await _fixedAssetService.GetAllAsync();
    //     return Ok(data);
    // }
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // Gọi hàm mới trong Service
        var result = await _fixedAssetService.GetAllWithDetailsAsync();
    
        return Ok(result);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAssetById(Guid id)
    {
        try 
        {
            var result = await _fixedAssetService.GetAssetByIdWithDetailAsync(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    [HttpPost]
    public async Task<IActionResult> Insert([FromBody] FixedAssetEntity entity)
    {
            var result = await _fixedAssetService.InsertServiceAsync(entity);
            return StatusCode(201, result);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] FixedAssetEntity entity)
    {
        entity.fixed_asset_id = id;
        var result = await _fixedAssetService.UpdateServiceAsync(id, entity);
        if (result > 0)
        {
            return Ok(result);
        }
        return StatusCode(StatusCodes.Status500InternalServerError, new { UserMsg = "Không thể cập nhật, vui lòng kiểm tra lại ID." });
    }
    [HttpGet("filter")]
    public async Task<IActionResult> GetFilter([FromQuery] FixedAssetFilterDto filterDto)
    {
        // Gọi Service xử lý
        var result = await _fixedAssetService.GetFilterAsync(filterDto);
        
        return Ok(result);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _fixedAssetService.DeleteServiceAsync(id);
        return Ok(result);
    }

    // 2. API XÓA NHIỀU
    [HttpDelete("batch")] // URL: DELETE api/v1/FixedAssets/batch
    public async Task<IActionResult> DeleteMany([FromBody] List<Guid> ids)
    {
        var result = await _fixedAssetService.DeleteManyAsync(ids);
        return Ok(result);
    }
    
    [HttpGet("new-code")]
    public async Task<IActionResult> GetNewCode()
    {
        try
        {
            var newCode = await _fixedAssetService.GetNewCodeAsync();
            return Ok(newCode); 
        }
        catch (Exception ex)
        {
            // Xử lý lỗi (ghi log...)
            return StatusCode(500, new { UserMsg = "Lỗi sinh mã mới", DevMsg = ex.Message });
        }
    }
    [HttpPost("{id}/replicate")]
    public async Task<IActionResult> Replicate(Guid id)
    {
        try
        {
            var result = await _fixedAssetService.ReplicateAssetAsync(id);
            
            if (result == null) 
                return NotFound(new { Message = "Không tìm thấy tài sản gốc để nhân bản." });

            // Trả về mã 201 Created cùng đối tượng vừa tạo
            return StatusCode(201, result);
        }
        catch (Exception ex)
        {
            // Trả về thông báo lỗi nếu có ngoại lệ xảy ra
            return BadRequest(new { Message = ex.Message });
        }
    }
}