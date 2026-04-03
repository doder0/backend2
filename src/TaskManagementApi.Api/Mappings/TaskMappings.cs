using TaskManagementApi.Api.DTOs;
using TaskManagementApi.Api.Entities;
using TaskManagementApi.Api.Enums;

namespace TaskManagementApi.Api.Mappings;

public static class TaskMappings
{
    public static TaskResponseDto ToResponseDto(this TaskItem task)
    {
        return new TaskResponseDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            DueDate = task.DueDate,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }

    public static TaskItem ToEntity(this CreateTaskDto dto, DateTime utcNow)
    {
        return new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = dto.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
            Status = dto.Status,
            Priority = dto.Priority,
            DueDate = dto.DueDate,
            CreatedAt = utcNow,
            UpdatedAt = utcNow
        };
    }

    public static void ApplyUpdate(this TaskItem task, UpdateTaskDto dto, DateTime utcNow)
    {
        task.Title = dto.Title.Trim();
        task.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();
        task.Status = dto.Status;
        task.Priority = dto.Priority;
        task.DueDate = dto.DueDate;
        task.UpdatedAt = utcNow;
    }
}
