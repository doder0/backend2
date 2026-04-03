using TaskManagementApi.Api.Enums;

namespace TaskManagementApi.Api.DTOs;

public sealed class TaskQueryParameters
{
    public TaskStatus? Status { get; set; }
    public TaskPriority? Priority { get; set; }
    public DateTime? DueBefore { get; set; }
    public DateTime? DueAfter { get; set; }
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; }
}
