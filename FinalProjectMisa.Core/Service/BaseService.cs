using System.Reflection;
using FinalProjectMisa.Core.Entities;
using FinalProjectMisa.Core.Exceptions;
using FinalProjectMisa.Core.Interface.Repository;
using FinalProjectMisa.Core.Interface.Service;
using FinalProjectMisa.Core.MisaAttribute;

namespace FinalProjectMisa.Core.Service;

public class BaseService<T> : IBaseService<T>
{
    protected readonly IBaseRepo<T> _baseRepo;

    public BaseService(IBaseRepo<T> baseRepo)
    {
        _baseRepo = baseRepo;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _baseRepo.GetAllAsync();
    }

    public async Task<int> InsertServiceAsync(T entity)
    {
        var properties = typeof(T).GetProperties();
    
        // Tìm property nào có gắn [MisaPrimaryKey]
        var pkProp = properties.FirstOrDefault(p => p.GetCustomAttribute<MISAPrimaryKey>() != null);
    
        if (pkProp != null && pkProp.PropertyType == typeof(Guid))
        {
            var currentId = (Guid)pkProp.GetValue(entity);
            if (currentId == Guid.Empty)
            {
                pkProp.SetValue(entity, Guid.NewGuid());
            }
        }

        if (entity is BaseEntity baseEntity)
        {
            baseEntity.created_date = DateTime.Now;
        }
        await ValidateData(entity);
        var result = await _baseRepo.InsertAsync(entity);
        return result;
    }

    public async Task<int> UpdateServiceAsync(Guid id, T entity)
    {
        await ValidateData(entity);
        if (entity is BaseEntity baseEntity)
        {
            baseEntity.modified_date = DateTime.Now;
        }
        var result = await _baseRepo.UpdateAsync(id, entity);
        return result;
    }

    public async Task<int> DeleteServiceAsync(Guid id)
    {
        return await _baseRepo.DeleteAsync(id);
    }
    /// <summary>
        /// Hàm Validate dùng chung (Master Validate)
        /// Chức năng:
        /// 1. Quét Attribute (Required, MaxLength) để check lỗi cơ bản
        /// 2. Gọi hàm Custom Validate để check nghiệp vụ riêng
        /// </summary>
        protected async Task ValidateData(T entity)
        { var properties = typeof(T).GetProperties();
            var errors = new List<string>(); // Chứa danh sách lỗi

            foreach (var prop in properties)
            {
                var propValue = prop.GetValue(entity); // Lấy giá trị của property

                // 1.1 Kiểm tra Attribute [MisaRequired]
                var requiredAttr = prop.GetCustomAttribute<MISARequired>();
                if (requiredAttr != null)
                {
                    // Nếu giá trị null hoặc rỗng -> Lỗi
                    if (propValue == null || string.IsNullOrWhiteSpace(propValue.ToString()))
                    {
                        errors.Add(requiredAttr.ErrorMsg);
                    }
                }

                // 1.2 Kiểm tra Attribute [MisaMaxLength]
                var maxLengthAttr = prop.GetCustomAttribute<MISAMaxLength>();
                if (maxLengthAttr != null && propValue != null)
                {
                    // Nếu độ dài vượt quá giới hạn -> Lỗi
                    if (propValue.ToString().Trim().Length > maxLengthAttr.Length)
                    {
                        errors.Add(maxLengthAttr.ErrorMsg);
                    }
                }
            }

            // Nếu có lỗi từ Attribute -> Ném Exception ngay (để Middleware bắt trả về 400)
            if (errors.Count > 0)
            {
                // Truyền error list vào để FE hiển thị
                throw new ValidateException("Dữ liệu đầu vào không hợp lệ", errors);
            }

            // --- BƯỚC 2: VALIDATE NGHIỆP VỤ (CUSTOM) ---
            // Gọi hàm ảo (virtual) để class con (FixedAssetService) tự xử lý logic riêng
            await ValidateCustom(entity);
        }
    /// <summary>
    /// Hàm ảo cho phép class con ghi đè để viết validate nghiệp vụ riêng.
    /// (Ví dụ: Check trùng mã, check tỷ lệ hao mòn...)
    /// </summary>
    protected virtual async Task ValidateCustom(T entity)
    {
        await Task.CompletedTask;
    }
}