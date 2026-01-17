using System.Text.Json;
using System.Text.RegularExpressions;
using FinalProjectMisa.Core.Dto;
using FinalProjectMisa.Core.Exceptions;
using MySqlConnector;

namespace FinalProjectMisa.Api.Middware;

public class FixedAssetMiddware
{
    private readonly RequestDelegate _next;

    public FixedAssetMiddware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        // 1. Khởi tạo giá trị mặc định cho lỗi hệ thống (500)
        var statusCode = StatusCodes.Status500InternalServerError;
        var errorResponse = new ErrorResponse
        {
            UserMsg = exception.Message,
            ErrorCode = "500",
            TraceId = context.TraceIdentifier
        };

        // 2. Kiểm tra nếu là lỗi Validate (400) - Do mình chủ động ném ra
        if (exception is ValidateException validateEx)
        {
            statusCode = StatusCodes.Status400BadRequest;
            errorResponse.ErrorCode = "400";
            errorResponse.UserMsg = validateEx.UserMsg;
            errorResponse.Errors = validateEx.Errors;   
        }
        else if (exception is MySqlException mySqlEx)
        {
            if (mySqlEx.Number == 1062) // Lỗi trùng lặp
            {
                statusCode = StatusCodes.Status400BadRequest;
                errorResponse.ErrorCode = "400";
                var match = Regex.Match(mySqlEx.Message, @"Duplicate entry '([^']*)'");
            
                if (match.Success)
                {
                    var duplicateCode = match.Groups[1].Value;
                    errorResponse.UserMsg = $"Mã tài sản {duplicateCode} đã tồn tại trong hệ thống.";
                }
                else
                {
                    errorResponse.UserMsg = "Mã tài sản đã tồn tại trong hệ thống.";
                }
            }
        }
        // 3. Trả về Client
        context.Response.StatusCode = statusCode;
            
        // Serialize sang JSON (dùng CamelCase cho chuẩn JS)
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
        };
            
        var json = JsonSerializer.Serialize(errorResponse, jsonOptions);
        await context.Response.WriteAsync(json);
    }
}