using FinalProjectMisa.Core.Dto;
using FinalProjectMisa.Core.Entities;
using FinalProjectMisa.Core.Exceptions;
using FinalProjectMisa.Core.Interface.Repository;
using FinalProjectMisa.Core.Interface.Service;

namespace FinalProjectMisa.Core.Service;

public class FixedAssetService : BaseService<FixedAssetEntity>, IFixedAssetService
{
    IFixedAssetRepo _fixedAssetRepo;

    public FixedAssetService(IBaseRepo<FixedAssetEntity> baseRepo, IFixedAssetRepo fixedAssetRepo) : base(baseRepo)
    {
        _fixedAssetRepo = fixedAssetRepo;
    }

    public async Task<IEnumerable<FixedAssetDto>> GetAllWithDetailsAsync()
    {
        var assets = await _fixedAssetRepo.GetAllWithDetailsAsync();
        return assets;
    }

    public async Task<FixedAssetDto> GetAssetByIdWithDetailAsync(Guid assetId)
    {
        var asset = await _fixedAssetRepo.GetAssetByIdWithDetailAsync(assetId);
        if (asset == null)
        {
            // Nên ném Exception chuẩn để Middleware bắt được 404
            throw new ValidateException("Không tìm thấy tài sản."); 
        }
        return asset;
    }

    public async Task<PagedResult<FixedAssetDto>> GetFilterAsync(FixedAssetFilterDto filterDto)
    {
        if (filterDto.PageNumber < 1)
        {
            filterDto.PageNumber = 1;
        }

        if (filterDto.PageSize > 100)
        {
            filterDto.PageSize = 100;
        }

        var result = await _fixedAssetRepo.GetFilterAsync(filterDto);

        return result;
    }

    public async Task<int> DeleteManyAsync(List<Guid> assetIds)
    {
        if (assetIds == null || assetIds.Count == 0)
        {
            throw new ValidateException("Danh sách tài sản cần xóa không được để trống.");
        }

        // 2. Gọi Repo để xóa
        // Lưu ý: BaseRepo cần có hàm DeleteManyAsync (mình sẽ nhắc ở dưới)
        var result = await _baseRepo.DeleteManyAsync(assetIds);
        
        return result;
    }

    public async Task<string> GetNewCodeAsync()
    {
        string prefix = "TS";
        var maxCode = await _baseRepo.GetNewCodeAsync();
        if (string.IsNullOrEmpty(maxCode))
        {
            return $"{prefix}00001";
        }

        if (maxCode.StartsWith(prefix))
        {
            var numberPart = maxCode.Substring(prefix.Length);
            
            if (long.TryParse(numberPart, out long number))
            {
                number++;
                return $"{prefix}{number.ToString("D6")}"; 
            }
        }
        return $"{prefix}00001";
    }

    public async Task<FixedAssetDto> ReplicateAssetAsync(Guid id)
    {
        string newCode = await GetNewCodeAsync();
        return await _fixedAssetRepo.ReplicateAssetAsync(id, newCode);
    }
    protected override async Task ValidateCustom(FixedAssetEntity entity)
    {
        var listErrors = new List<string>();
        Guid? excludeId = null;

        // Nếu ID có giá trị và khác Guid rỗng -> Đây là trường hợp Sửa (Update)
        // Cần lấy ID này để loại trừ chính nó khi check trùng
        if (entity.fixed_asset_id != Guid.Empty && entity.fixed_asset_id != null)
        {
            excludeId = entity.fixed_asset_id;
        }

        // Gọi Repo kiểm tra
        var isDuplicate = await _baseRepo.CheckDuplicateCodeAsync(entity.fixed_asset_code, excludeId);

        if (isDuplicate)
        {
            throw new ValidateException($"Mã tài sản {entity.fixed_asset_code} đã tồn tại trong hệ thống.");
        }

        // --- 2. KIỂM TRA NGHIỆP VỤ HAO MÒN ---

        // Validate 1: Tỷ lệ hao mòn phải bằng (1 / Số năm sử dụng) * 100
        // Cần kiểm tra usage_years > 0 để tránh lỗi chia cho 0
        if (entity.usage_years > 0)
        {
            decimal expectedRate = (1m / (decimal)entity.usage_years) * 100;
            expectedRate = Math.Round(expectedRate, 2);
            decimal inputRate = Math.Round(entity.depreciation_rate, 2);

            if (expectedRate != inputRate)
            {
                listErrors.Add("Tỷ lệ hao mòn phải bằng 1/Số năm sử dụng");
            }
        }

        // --- CHECK 3: GIÁ TRỊ HAO MÒN > NGUYÊN GIÁ ---
        if (entity.depreciation_value > entity.cost)
        {
            listErrors.Add("Hao mòn năm phải nhỏ hơn hoặc bằng nguyên giá");
        }

        // --- BƯỚC CUỐI: NẾU CÓ LỖI THÌ MỚI NÉM RA ---
        if (listErrors.Count > 0)
        {
            // Dùng constructor thứ 2 của ValidateException (nhận vào List error)
            // UserMsg chung chung, còn Errors chứa chi tiết từng dòng
            throw new ValidateException("Dữ liệu đầu vào không hợp lệ", listErrors);
        }
    }
}