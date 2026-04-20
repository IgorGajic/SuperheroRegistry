using SuperheroRegistry.Domain.Exceptions;
using System.Net;

namespace SuperheroRegistry.API.Middleware;

/// <summary>
/// Global exception handler middleware that catches unhandled exceptions
/// and returns appropriate HTTP responses.
/// </summary>
public class GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex, _logger);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse();

        switch (exception)
        {
            case KeyNotFoundException ex:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Message = ex.Message;
                response.ErrorCode = "NOT_FOUND";
                break;

            case UnauthorizedAccessException ex:
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                response.Message = ex.Message;
                response.ErrorCode = "FORBIDDEN";
                break;

            case ArgumentException ex:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = ex.Message;
                response.ErrorCode = "INVALID_ARGUMENT";
                break;

            case InvalidOperationException ex:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = ex.Message;
                response.ErrorCode = "INVALID_OPERATION";
                break;

            case DomainException ex:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = ex.Message;
                response.ErrorCode = "DOMAIN_ERROR";
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = "An unexpected error occurred. Our backend team is notified. Please try again later.";
                response.ErrorCode = "INTERNAL_SERVER_ERROR";
                // Log with Alert level to trigger instrumentation/monitoring alerts
                logger.LogError(exception, "ALERT: Internal server error detected. ErrorCode: {ErrorCode}", response.ErrorCode);
                break;
        }

        return context.Response.WriteAsJsonAsync(response);
    }
}

public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
