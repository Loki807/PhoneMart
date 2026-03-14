using System.Net;
using System.Text.Json;

namespace PhoneMart.API.Middleware;

/// <summary>
/// GLOBAL ERROR HANDLING MIDDLEWARE
/// 
/// Why? Without this, when an exception happens:
///   - Raw stack traces go to the frontend (security risk!)
///   - No consistent error format (sometimes HTML, sometimes JSON)
///   - 500 errors with no useful message
/// 
/// With this middleware:
///   - ALL exceptions are caught here
///   - Clean JSON error response { "message": "..." }
///   - No stack traces leaked
///   - Consistent format for frontend to parse
/// 
/// How middleware works:
///   Every HTTP request passes through this before reaching the controller.
///   If the controller throws an exception, it bubbles up to here.
///   We catch it and return a friendly error response.
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            // Try to process the request normally
            await _next(context);
        }
        catch (Exception ex)
        {
            // Log the full exception (for debugging)
            _logger.LogError(ex, "An error occurred: {Message}", ex.Message);

            // Return a clean JSON error to the frontend
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var errorResponse = new
            {
                message = ex.Message,
                statusCode = context.Response.StatusCode
            };

            var json = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(json);
        }
    }
}
