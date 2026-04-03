namespace TaskManagementApi.Api.DTOs;

public sealed class TaskStatsDto
{
    public int TotalCount { get; set; }
    public int OpenCount { get; set; }
    public int InProgressCount { get; set; }
    public int CompletedCount { get; set; }
    public int LowPriorityCount { get; set; }
    public int MediumPriorityCount { get; set; }
    public int HighPriorityCount { get; set; }
    public int OverdueCount { get; set; }
    public int CompletedTasksCount { get; set; }
    public int OpenTasksCount { get; set; }
}
