using Dapper;
using FinalProjectMisa.Core.Dto;
using FinalProjectMisa.Core.Entities;
using FinalProjectMisa.Core.Interface.Repository;
using Microsoft.Extensions.Configuration;

namespace FinalProjectMisa.Infrastructure.Repository;

public class DepartmentRepo :  BaseRepo<DepartmentEntity>, IDepartmentRepo
{
    public DepartmentRepo(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<IEnumerable<DepartmentDto>> GetDepartment()
    {
        using var connection = CreateConnection();

        var sql = @$"
                SELECT {nameof(DepartmentEntity.department_id)},      
                       {nameof(DepartmentEntity.department_code)},
                    {nameof(DepartmentEntity.department_name)}
                FROM department
                ORDER BY {nameof(DepartmentEntity.department_code)};
            ";

        return await connection.QueryAsync<DepartmentDto>(sql);
    }

}