using Dapper;
using FinalProjectMisa.Core.Dto;
using FinalProjectMisa.Core.Entities;
using FinalProjectMisa.Core.Interface.Repository;
using Microsoft.Extensions.Configuration;

namespace FinalProjectMisa.Infrastructure.Repository;

public class FixedAssetRepo : BaseRepo<FixedAssetEntity>, IFixedAssetRepo
{
    public FixedAssetRepo(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<IEnumerable<FixedAssetDto>> GetAllWithDetailsAsync()
    {
        {
            using var connection = CreateConnection();
            var sql = $@"{GetBaseSelectQuery()} 
                     ORDER BY {TableAlias}.{nameof(FixedAssetEntity.created_date)} DESC;";

            return await connection.QueryAsync<FixedAssetDto>(sql);
        }
    }

public async Task<PagedResult<FixedAssetDto>> GetFilterAsync(FixedAssetFilterDto filterDto)
{
    using var connection = CreateConnection();

    // ... (Phần code xử lý Dictionary và WhereClause giữ nguyên) ...
    var conditions = new Dictionary<string, object>();
    var whereSql = new List<string>();
    var parameters = new DynamicParameters();

    // 1. Build Conditions
    if (filterDto.DepartmentId.HasValue)
    {
        conditions.Add(nameof(FixedAssetEntity.department_id), filterDto.DepartmentId);
    }
    if (filterDto.FixedAssetCategoryId.HasValue)
    {
        conditions.Add(nameof(FixedAssetEntity.fixed_asset_category_id), filterDto.FixedAssetCategoryId);
    }
    foreach (var item in conditions)
    {
        whereSql.Add($"{TableAlias}.{item.Key} = @{item.Key}");
        parameters.Add($"@{item.Key}", item.Value);
    }
    if (!string.IsNullOrEmpty(filterDto.Keyword))
    {
        var colCode = nameof(FixedAssetEntity.fixed_asset_code);
        var colName = nameof(FixedAssetEntity.fixed_asset_name);
        whereSql.Add($"({TableAlias}.{colCode} LIKE @Keyword OR {TableAlias}.{colName} LIKE @Keyword)");
        parameters.Add("@Keyword", $"%{filterDto.Keyword}%");
    }

    var whereClause = whereSql.Any() ? "WHERE " + string.Join(" AND ", whereSql) : "";

    // 2. Chuẩn bị SQL
    var sqlData = $@"
        {GetBaseSelectQuery()}
        {whereClause}
        ORDER BY {TableAlias}.{nameof(FixedAssetEntity.created_date)} DESC
        LIMIT @Limit OFFSET @Skip;";

    var sqlCount = $"SELECT COUNT(*) FROM fixed_asset {TableAlias} {whereClause}";

    // Thêm tham số phân trang
    var skip = (filterDto.PageNumber - 1) * filterDto.PageSize;
    parameters.Add("@Limit", filterDto.PageSize);
    parameters.Add("@Skip", skip);
    
    // Bước 1: Lấy danh sách dữ liệu trước
    var assets = await connection.QueryAsync<FixedAssetDto>(sqlData, parameters);

    // Bước 2: Đếm tổng số bản ghi sau
    // (Connection lúc này đã rảnh tay sau khi chạy xong lệnh trên)
    var totalRecords = await connection.ExecuteScalarAsync<int>(sqlCount, parameters);

    return new PagedResult<FixedAssetDto>
    {
        PageNumber = filterDto.PageNumber,
        PageSize = filterDto.PageSize,
        TotalRecords = totalRecords,
        Data = assets
    };
}

public async Task<FixedAssetDto> GetAssetByIdWithDetailAsync(Guid assetId)
{
    using var connection = CreateConnection();

    // Tái sử dụng Base Query + thêm WHERE ID
    var sql = $@"{GetBaseSelectQuery()} 
                     WHERE {TableAlias}.{nameof(FixedAssetEntity.fixed_asset_id)} = @Id";

    return await connection.QueryFirstOrDefaultAsync<FixedAssetDto>(sql, new { Id = assetId });
}
/// <summary>
    /// Hàm build câu query cơ bản: SELECT ... FROM ... JOIN ...
    /// Mục đích: Dùng chung cho cả GetAll, GetById, GetFilter để không phải viết lại code JOIN nhiều lần.
    /// </summary>
    /// <summary>
    /// Hàm dùng chung để lấy câu SELECT + JOIN chuẩn
    /// </summary>
    private const string TableAlias = "fa";
    protected string GetBaseSelectQuery()
    {
        const string tAsset = TableAlias; // Sử dụng alias chung
        const string tDept = "d";
        const string tCat = "c";

        return $@"
        SELECT 
            {tAsset}.*, 
            {tDept}.{nameof(DepartmentEntity.department_name)},
            {tDept}.{nameof(DepartmentEntity.department_code)},
            {tCat}.{nameof(FixedAssetCategoryEntity.fixed_asset_category_code)},
            {tCat}.{nameof(FixedAssetCategoryEntity.fixed_asset_category_name)}
        FROM fixed_asset {tAsset}
        LEFT JOIN department {tDept} 
            ON {tAsset}.{nameof(FixedAssetDto.department_id)} = {tDept}.{nameof(DepartmentEntity.department_id)}
        LEFT JOIN fixed_asset_category {tCat} 
            ON {tAsset}.{nameof(FixedAssetDto.fixed_asset_category_id)} = {tCat}.{nameof(FixedAssetCategoryEntity.fixed_asset_category_id)}
        ";
    }
    public async Task<string> GetMaxCodeAsync()
    {
        using var connection = CreateConnection();
    
        // Quy tắc tìm mã lớn nhất:
        // 1. Chỉ lấy các mã bắt đầu bằng "TS"
        // 2. Sắp xếp độ dài giảm dần (để số nhiều chữ số lên đầu)
        // 3. Sắp xếp giá trị giảm dần
        // 4. Lấy 1 bản ghi đầu tiên
        var sql = $"SELECT {_tableName}_code FROM {_tableName} " +
                  $"WHERE {_tableName}_code LIKE 'TS%' " +
                  $"ORDER BY LENGTH({_tableName}_code) DESC, {_tableName}_code DESC LIMIT 1";

        var maxCode = await connection.QueryFirstOrDefaultAsync<string>(sql);
        return maxCode;
    }
    public async Task<FixedAssetDto> ReplicateAssetAsync(Guid oldId, string newCode)
    {
        using var connection = CreateConnection();
        var newId = Guid.NewGuid();

        // 1. Chỉ định tên bảng an toàn (Nên hardcode hoặc lấy từ hằng số nội bộ)
        // Tránh để người dùng truyền tên bảng từ URL/Input vào biến _tableName
        var safeTableName = $"`{_tableName}`"; // Dùng dấu ` cho MySQL

        var sql = $@"
INSERT INTO {safeTableName} (
    fixed_asset_id, 
    fixed_asset_code, 
    fixed_asset_name, 
    fixed_asset_category_id, 
    department_id, 
    quantity, 
    cost, 
    depreciation_value,
    purchase_date,
    tracked_year,
    production_year
)
SELECT 
    @NewId, 
    @NewCode, 
    fixed_asset_name, 
    fixed_asset_category_id, 
    department_id, 
    quantity, 
    cost, 
    depreciation_value,
    purchase_date,
    tracked_year,
    production_year
FROM {safeTableName} 
WHERE fixed_asset_id = @OldId;

-- 2. TRẢ VỀ DỮ LIỆU KÈM TÊN (JOIN để Frontend hiển thị được ngay)
SELECT 
    a.*, 
    d.department_name, 
d.department_code, 
    c.fixed_asset_category_name 
, c.fixed_asset_category_code
FROM {safeTableName} a
LEFT JOIN department d ON a.department_id = d.department_id
LEFT JOIN fixed_asset_category c ON a.fixed_asset_category_id = c.fixed_asset_category_id
WHERE a.fixed_asset_id = @NewId;";

        // Dapper sẽ tự động map các giá trị trong object này vào các tham số @ 
        // một cách an toàn nhất chống SQL Injection.
        return await connection.QueryFirstOrDefaultAsync<FixedAssetDto>(sql, new {
            NewId = newId,
            NewCode = newCode,
            OldId = oldId
        });
    }
}