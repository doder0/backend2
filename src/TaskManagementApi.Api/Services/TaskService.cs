using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Api.Data;
using TaskManagementApi.Api.DTOs;
using TaskManagementApi.Api.Entities;
using TaskManagementApi.Api.Exceptions;
using TaskManagementApi.Api.Mappings;
using TaskManagementApi.Api.Enums;

namespace TaskManagementApi.Api.Services;

public sealed class TaskService : ITaskService
{
    private static readonly HashSet<string> AllowedSortFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "title",
        "status",
        "priority",
        "dueDate",
        "createdAt",
        "updatedAt"
    };

    private static readonly HashSet<string> AllowedSortOrders = new(StringComparer.OrdinalIgnoreCase)
    {
        "asc",
        "desc"
    };

    private readonly ApplicationDbContext _context;

    public TaskService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<TaskResponseDto>> GetTasksAsync(TaskQueryParameters queryParameters, CancellationToken cancellationToken = default)
    {
        IQueryable<TaskItem> query = _context.Tasks.AsNoTracking();

        if (queryParameters.Status.HasValue)
        {
            query = query.Where(task => task.Status == queryParameters.Status.Value);
        }

        if (queryParameters.Priority.HasValue)
        {
            query = query.Where(task => task.Priority == queryParameters.Priority.Value);
        }

        if (queryParameters.DueBefore.HasValue)
        {
            query = query.Where(task => task.DueDate <= queryParameters.DueBefore.Value);
        }

        if (queryParameters.DueAfter.HasValue)
        {
            query = query.Where(task => task.DueDate >= queryParameters.DueAfter.Value);
        }

        query = ApplySorting(query, queryParameters.SortBy, queryParameters.SortOrder);

        return await query
            .Select(task => task.ToResponseDto())
            .ToListAsync(cancellationToken);
    }

    public async Task<TaskResponseDto> GetTaskByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var task = await _context.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        if (task is null)
        {
            throw new NotFoundException($"Task '{id}' was not found.");
        }

        return task.ToResponseDto();
    }

    public async Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto dto, CancellationToken cancellationToken = default)
    {
        EnsureDueDateIsInFuture(dto.DueDate);

        var utcNow = DateTime.UtcNow;
        var task = dto.ToEntity(utcNow);

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync(cancellationToken);

        return task.ToResponseDto();
    }

    public async Task<TaskResponseDto> UpdateTaskAsync(Guid id, UpdateTaskDto dto, CancellationToken cancellationToken = default)
    {
        EnsureDueDateIsInFuture(dto.DueDate);

        var task = await _context.Tasks.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        if (task is null)
        {
            throw new NotFoundException($"Task '{id}' was not found.");
        }

        task.ApplyUpdate(dto, DateTime.UtcNow);

        await _context.SaveChangesAsync(cancellationToken);
        return task.ToResponseDto();
    }

    public async Task DeleteTaskAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var task = await _context.Tasks.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        if (task is null)
        {
            throw new NotFoundException($"Task '{id}' was not found.");
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<TaskStatsDto> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var totalCount = await _context.Tasks.CountAsync(cancellationToken);
        var openCount = await _context.Tasks.CountAsync(task => task.Status == TaskStatus.Open, cancellationToken);
        var inProgressCount = await _context.Tasks.CountAsync(task => task.Status == TaskStatus.InProgress, cancellationToken);
        var completedCount = await _context.Tasks.CountAsync(task => task.Status == TaskStatus.Completed, cancellationToken);
        var lowPriorityCount = await _context.Tasks.CountAsync(task => task.Priority == TaskPriority.Low, cancellationToken);
        var mediumPriorityCount = await _context.Tasks.CountAsync(task => task.Priority == TaskPriority.Medium, cancellationToken);
        var highPriorityCount = await _context.Tasks.CountAsync(task => task.Priority == TaskPriority.High, cancellationToken);
        var overdueCount = await _context.Tasks.CountAsync(task => task.Status != TaskStatus.Completed && task.DueDate < DateTime.UtcNow, cancellationToken);

        return new TaskStatsDto
        {
            TotalCount = totalCount,
            OpenCount = openCount,
            InProgressCount = inProgressCount,
            CompletedCount = completedCount,
            LowPriorityCount = lowPriorityCount,
            MediumPriorityCount = mediumPriorityCount,
            HighPriorityCount = highPriorityCount,
            OverdueCount = overdueCount,
            CompletedTasksCount = completedCount,
            OpenTasksCount = openCount
        };
    }

    private static void EnsureDueDateIsInFuture(DateTime dueDate)
    {
        if (dueDate <= DateTime.UtcNow)
        {
            throw new BadRequestException("DueDate must be in the future.");
        }
    }

    private static IQueryable<TaskItem> ApplySorting(IQueryable<TaskItem> query, string? sortBy, string? sortOrder)
    {
        var field = string.IsNullOrWhiteSpace(sortBy) ? "createdAt" : sortBy.Trim();
        var order = string.IsNullOrWhiteSpace(sortOrder) ? "desc" : sortOrder.Trim();

        if (!AllowedSortFields.Contains(field))
        {
            throw new BadRequestException("sortBy must be one of: title, status, priority, dueDate, createdAt, updatedAt.");
        }

        if (!AllowedSortOrders.Contains(order))
        {
            throw new BadRequestException("sortOrder must be asc or desc.");
        }

        var descending = order.Equals("desc", StringComparison.OrdinalIgnoreCase);

        return field.ToLowerInvariant() switch
        {
            "title" => descending ? query.OrderByDescending(task => task.Title) : query.OrderBy(task => task.Title),
            "status" => descending ? query.OrderByDescending(task => task.Status) : query.OrderBy(task => task.Status),
            "priority" => descending ? query.OrderByDescending(task => task.Priority) : query.OrderBy(task => task.Priority),
            "duedate" => descending ? query.OrderByDescending(task => task.DueDate) : query.OrderBy(task => task.DueDate),
            "createdat" => descending ? query.OrderByDescending(task => task.CreatedAt) : query.OrderBy(task => task.CreatedAt),
            "updatedat" => descending ? query.OrderByDescending(task => task.UpdatedAt) : query.OrderBy(task => task.UpdatedAt),
            _ => query
        };
    }
}
