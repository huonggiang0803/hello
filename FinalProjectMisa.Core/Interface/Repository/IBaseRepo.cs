using FinalProjectMisa.Core.Dto;

namespace FinalProjectMisa.Core.Interface.Repository;

public interface IBaseRepo<T>
{
   /// <summary>
        /// Lấy tất cả bản ghi
        /// Nghiệp vụ: Hiển thị danh sách tài sản lên Grid 
        /// </summary>
        /// <returns>Danh sách thực thể</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Lấy thông tin chi tiết một bản ghi theo ID
        /// Nghiệp vụ: Hiển thị thông tin lên form chi tiết để Sửa 
        /// </summary>
        /// <param name="entityId">ID của bản ghi</param>
        /// <returns>Bản ghi duy nhất hoặc null nếu không tìm thấy</returns>
        Task<T> GetByIdAsync(Guid entityId);

        /// <summary>
        /// Thêm mới một bản ghi
        /// Nghiệp vụ: Chức năng Thêm mới tài sản 
        /// </summary>
        /// <param name="entity">Dữ liệu bản ghi cần thêm</param>
        /// <returns>Số bản ghi bị ảnh hưởng (Thường là 1)</returns>
        Task<int> InsertAsync(T entity);

        /// <summary>
        /// Cập nhật thông tin bản ghi
        /// Nghiệp vụ: Chức năng Sửa tài sản 
        /// </summary>
        /// <param name="entityId">ID của bản ghi cần sửa</param>
        /// <param name="entity">Dữ liệu mới</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        Task<int> UpdateAsync(Guid entityId, T entity);

        /// <summary>
        /// Xóa một bản ghi theo ID
        /// Nghiệp vụ: Chức năng Xóa tài sản (xóa đơn) 
        /// </summary>
        /// <param name="entityId">ID bản ghi cần xóa</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        Task<int> DeleteAsync(Guid entityId);

        /// <summary>
        /// Xóa nhiều bản ghi cùng lúc
        /// Nghiệp vụ: Cho phép chọn nhiều bằng Ctrl, Shift để xóa nhiều 
        /// </summary>
        /// <param name="entityIds">Danh sách ID các bản ghi cần xóa</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        Task<int> DeleteManyAsync(List<Guid> entityIds);

        /// <summary>
        /// Kiểm tra trùng mã
        /// Nghiệp vụ: Validate "Không được trùng mã tài sản đã có trong danh sách" 
        /// </summary>
        /// <param name="entityCode">Mã cần kiểm tra</param>
        /// <returns>True: Đã tồn tại | False: Chưa tồn tại</returns>
        Task<bool> CheckDuplicateCodeAsync(string entityCode , Guid? excludeId = null);

        /// <summary>
        /// Lấy mã mới tự động tăng
        /// Nghiệp vụ: Khi nhân bản hoặc thêm mới, mã tài sản cần tự động tăng/gợi ý 
        /// </summary>
        /// <returns>Chuỗi mã mới (Ví dụ: TS00002)</returns>
        Task<string> GetNewCodeAsync();
}