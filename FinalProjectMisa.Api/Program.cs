using System.Data;
using FinalProjectMisa.Api.Middware;
using FinalProjectMisa.Core.Interface.Repository;
using FinalProjectMisa.Core.Interface.Service;
using FinalProjectMisa.Core.Service;
using FinalProjectMisa.Infrastructure.Repository;
using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") 
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

// Add services to the container.
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddScoped<IDbConnection>(sp => new MySqlConnection(connectionString));
builder.Services.AddScoped(typeof(IBaseRepo<>), typeof(BaseRepo<>));
builder.Services.AddScoped<IFixedAssetRepo, FixedAssetRepo>();
builder.Services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));
builder.Services.AddScoped<IFixedAssetService, FixedAssetService>();

builder.Services.AddScoped<IDepartmentRepo, DepartmentRepo>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();

builder.Services.AddScoped<IFixedAssetCategoryRepo, FixedAssetCategoryRepo>();
builder.Services.AddScoped<IFixedAssetCategoryService, FixedAssetCategoryService>();
// Configure the HTTP request pipeline.


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowVueApp");
app.UseMiddleware<FixedAssetMiddware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();