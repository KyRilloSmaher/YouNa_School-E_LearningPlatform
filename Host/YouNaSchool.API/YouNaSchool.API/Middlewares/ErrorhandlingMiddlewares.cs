using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Shared.Application.RESULT_PATTERN;
using System.Net;
using System.Text.Json;

namespace YouNaSchool.API.Middlewares;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;

    public ErrorHandlerMiddleware(RequestDelegate next,
                                  ILogger<ErrorHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var errorsBag = new Dictionary<string, List<string>>();
        var message = "An unexpected error occurred.";

        switch (exception)
        {
            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized;
                errorsBag["Authentication"] = new() { "Unauthorized access." };
                message = "Unauthorized.";
                break;

            case ValidationException validationException:
                statusCode = HttpStatusCode.UnprocessableEntity;
                message = "Validation failed.";

                errorsBag = validationException.Errors
                    .GroupBy(e => string.IsNullOrWhiteSpace(e.PropertyName) ? "General" : e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).Distinct().ToList()
                    );
                break;

            case KeyNotFoundException ex:
                statusCode = HttpStatusCode.NotFound;
                errorsBag["NotFound"] = new() { ex.Message };
                message = "Resource not found.";
                break;

            case DbUpdateException dbException:
                statusCode = HttpStatusCode.BadRequest;
                var dbMsg = dbException.InnerException?.Message ?? dbException.Message;
                errorsBag["Database"] = new() { dbMsg };
                message = "Database operation failed.";
                break;

            default:
                errorsBag["Error"] = new() { exception.Message };
                break;
        }

        // 🔥 Structured logging (IMPORTANT)
        _logger.LogError(exception,
            "Unhandled exception occurred. StatusCode: {StatusCode}",
            (int)statusCode);

        var responseModel = Result<string>.Failure(errorsBag.ToString() ,code:statusCode);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(responseModel, options));
    }
}