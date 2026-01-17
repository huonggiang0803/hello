using System.Data;
using System.Reflection;
using Dapper;
using FinalProjectMisa.Core.Interface.Repository;
using FinalProjectMisa.Core.MisaAttribute;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace FinalProjectMisa.Infrastructure.Repository;

/// <summary>
/// Repository base dùng chung cho các thực thể
/// Thực hiện các thao tác CRUD cơ bản với Database
/// </summary>
/// <typeparam name="T">Kiểu thực thể</typeparam>
public class BaseRepo<T> : IBaseRepo<T>
{
    /// <summary>
    /// Chuỗi kết nối Database
    /// </summary>
    protected readonly string _connectionString;
    /// <summary>
    /// Tên bảng trong Database
    /// </summary>
    protected string _tableName;
    /// <summary>
    /// Tên khóa chính của bảng
    /// </summary>
    protected string _primaryKey;
    /// <summary>
    /// Khởi tạo repository với cấu hình từ appsettings
    /// </summary>
    /// <param name="configuration">Đối tượng cấu hình</param>
    public BaseRepo( IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");

        // 1. Lấy tên bảng
        var tableNameAttribute = typeof(T).GetCustomAttribute<MISATableName>();
        if (tableNameAttribute != null)
        {
            _tableName = tableNameAttribute.TableName;
        }
        else
        {
            _tableName = typeof(T).Name;
        }
        var properties = typeof(T).GetProperties();
    
        // Tìm property có gắn attribute [MISAPrimaryKey]
        var primaryKeyProperty = properties.FirstOrDefault(p => p.GetCustomAttribute<MISAPrimaryKey>() != null);

        if (primaryKeyProperty != null)
        {
            _primaryKey = primaryKeyProperty.Name; // Ví dụ: "fixed_asset_id"
        }
        else
        {
            // Fallback nếu quên gắn attribute (nhưng tốt nhất là nên gắn)
            _primaryKey = $"{_tableName}_id"; 
        }
    }
    /// <summary>
    /// Khởi tạo repository với chuỗi kết nối
    /// </summary>
    /// <param name="connectionString">Chuỗi kết nối Database</param>
    public BaseRepo(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// Khởi tạo kết nối tới Database
    /// </summary>
    protected IDbConnection CreateConnection()
    {
        return new MySqlConnection(_connectionString);
    }
    /// <summary>
    /// Lấy toàn bộ danh sách bản ghi
    /// </summary>
    /// <returns>Danh sách thực thể</returns>
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        using var connection = CreateConnection();
        var sql = $"SELECT * FROM {_tableName}";
        var result = await connection.QueryAsync<T>(sql);
        return result;
    }

    /// <summary>
    /// Lấy thông tin bản ghi theo Id
    /// </summary>
    /// <param name="entityId">Id của thực thể</param>
    /// <returns>Thực thể tương ứng hoặc null</returns>
    public async Task<T> GetByIdAsync(Guid entityId)
    {
        using (var connection = CreateConnection())
        {
            var sql = $"SELECT * FROM {_tableName} WHERE {_primaryKey} = @id";
            var result = await connection.QueryFirstOrDefaultAsync<T>(sql, new { id = entityId });
            return result;
        }
    }
    /// <summary>
    /// Thêm mới một bản ghi
    /// </summary>
    /// <param name="entity">Đối tượng cần thêm</param>
    /// <returns>Số bản ghi bị ảnh hưởng</returns>
    public async Task<int> InsertAsync(T entity)
    {
        using var connection = CreateConnection();

        var props = typeof(T).GetProperties();
        var columns = string.Join(", ", props.Select(p => p.Name));
        var values  = string.Join(", ", props.Select(p => "@" + p.Name));

        var sql = $"INSERT INTO {_tableName} ({columns}) VALUES ({values})";

        return await connection.ExecuteAsync(sql, entity);
    }

    /// <summary>
    /// Cập nhật thông tin bản ghi
    /// </summary>
    /// <param name="entityId">Id của thực thể</param>
    /// <param name="entity">Đối tượng cập nhật</param>
    /// <returns>Số bản ghi bị ảnh hưởng</returns>
    public async Task<int> UpdateAsync(Guid entityId, T entity)
    {
        using var connection = CreateConnection();
        var props = typeof(T).GetProperties().Where(p => p.Name != _primaryKey);

        var setClause = string.Join(", ", props.Select(p => $"{p.Name} = @{p.Name}"));

        var sql = $"UPDATE {_tableName} SET {setClause} WHERE {_primaryKey} = @id";

        var parameters = new DynamicParameters(entity);
        parameters.Add("@id", entityId);
        var rowsAffected = await connection.ExecuteAsync(sql, parameters);
        return rowsAffected;
    }
    /// <summary>
    /// Xóa một bản ghi theo Id
    /// </summary>
    /// <param name="entityId">Id của thực thể</param>
    /// <returns>Số bản ghi bị ảnh hưởng</returns>
    public async Task<int> DeleteAsync(Guid entityId)
    {
        using (var connection = CreateConnection())
        {
            var sql = $"DELETE FROM {_tableName} WHERE {_primaryKey} = @id";
            var rowsAffected = await connection.ExecuteAsync(sql, new { id = entityId });
            return rowsAffected;
        }
    }
    /// <summary>
    /// Xóa nhiều bản ghi theo danh sách Id
    /// </summary>
    /// <param name="entityIds">Danh sách Id cần xóa</param>
    /// <returns>Số bản ghi bị ảnh hưởng</returns>
    public async Task<int> DeleteManyAsync(List<Guid> entityIds)
    {
        using (var connection = CreateConnection())
        {
            var sql = $"DELETE FROM {_tableName} WHERE {_primaryKey} IN @ids";
                
            var rowsAffected = await connection.ExecuteAsync(sql, new { ids = entityIds });
            return rowsAffected;
        }
    }
    /// <summary>
    /// Kiểm tra trùng mã thực thể
    /// </summary>
    /// <param name="entityCode">Mã thực thể</param>
    /// <returns>
    /// True: mã đã tồn tại <br/>
    /// False: mã chưa tồn tại
    /// </returns>
    public async Task<bool> CheckDuplicateCodeAsync(string entityCode, Guid? excludeId = null)
    {
        using var connection = CreateConnection();

        // Giả định quy tắc tên cột mã = Tên bảng + "_code" (ví dụ: fixed_asset_code)
        // Nếu Entity bạn đặt tên khác, bạn nên tạo thêm Attribute [MisaCode] để lấy chính xác.
        var codeColumnName = $"{_tableName}_code"; 

        var sql = $"SELECT {codeColumnName} FROM {_tableName} WHERE {codeColumnName} = @code";
        var parameters = new DynamicParameters();
        parameters.Add("@code", entityCode);

        // Nếu có excludeId -> Đây là Update -> Thêm điều kiện loại trừ chính nó
        if (excludeId.HasValue)
        {
            sql += $" AND {_primaryKey} <> @id";
            parameters.Add("@id", excludeId);
        }

        // Chỉ lấy 1 bản ghi đầu tiên tìm thấy
        var result = await connection.QueryFirstOrDefaultAsync<string>(sql, parameters);
        return result != null; // Trả về True nếu tìm thấy (bị trùng)
    }
    /// <summary>
    /// Sinh mã mới cho thực thể
    /// </summary>
    /// <returns>Mã mới</returns>
    public async Task<string> GetNewCodeAsync()
    {
        using var connection = CreateConnection();
        var codeColumnName = $"{_tableName}_code"; 
        var sql = $"SELECT {codeColumnName} FROM {_tableName} " +
                  $"ORDER BY LENGTH({codeColumnName}) DESC, {codeColumnName} DESC LIMIT 1";

        var maxCode = await connection.QueryFirstOrDefaultAsync<string>(sql);
        
        return maxCode;
    }
}