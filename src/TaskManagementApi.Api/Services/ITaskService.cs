using TaskManagementApi.Api.DTOs;

namespace TaskManagementApi.Api.Services;

public interface ITaskService
{
    Task<IReadOnlyList<TaskResponseDto>> GetTasksAsync(TaskQueryParameters queryParameters, CancellationToken cancellationToken = default);
    Task<TaskResponseDto> GetTaskByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto dto, CancellationToken cancellationToken = default);
    Task<TaskResponseDto> UpdateTaskAsync(Guid id, UpdateTaskDto dto, CancellationToken cancellationToken = default);
    Task DeleteTaskAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TaskStatsDto> GetStatsAsync(CancellationToken cancellationToken = default);
}
