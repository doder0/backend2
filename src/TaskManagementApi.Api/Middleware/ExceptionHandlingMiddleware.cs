using System.Text.Json;
using TaskManagementApi.Api.DTOs.Common;
using TaskManagementApi.Api.Exceptions;

namespace TaskManagementApi.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            await WriteErrorAsync(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (BadRequestException ex)
        {
            await WriteErrorAsync(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteErrorAsync(context, StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, int statusCode, string message)
    {
        if (context.Response.HasStarted)
        {
            throw new InvalidOperationException("The response has already started.");
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var payload = new ApiErrorResponse(
            statusCode,
            message,
            null,
            context.TraceIdentifier);

        await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}
