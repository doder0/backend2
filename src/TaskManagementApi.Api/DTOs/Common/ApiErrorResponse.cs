namespace TaskManagementApi.Api.DTOs.Common;

public sealed class ApiErrorResponse
{
    public ApiErrorResponse(int statusCode, string message, object? errors, string traceId)
    {
        StatusCode = statusCode;
        Message = message;
        Errors = errors;
        TraceId = traceId;
    }

    public int StatusCode { get; }
    public string Message { get; }
    public object? Errors { get; }
    public string TraceId { get; }
}
