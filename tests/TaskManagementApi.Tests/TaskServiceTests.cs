using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Api.Data;
using TaskManagementApi.Api.DTOs;
using TaskManagementApi.Api.Entities;
using TaskManagementApi.Api.Enums;
using TaskManagementApi.Api.Services;

namespace TaskManagementApi.Tests;

public sealed class TaskServiceTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    [Fact]
    public async Task CreateTaskAsync_CreatesTask()
    {
        await using var context = CreateContext();
        var service = new TaskService(context);

        var dto = new CreateTaskDto
        {
            Title = "Write tests",
            Description = "Cover service behavior.",
            Status = TaskStatus.Open,
            Priority = TaskPriority.High,
            DueDate = DateTime.UtcNow.AddDays(2)
        };

        var created = await service.CreateTaskAsync(dto);

        Assert.Equal("Write tests", created.Title);
        Assert.Equal(TaskStatus.Open, created.Status);
        Assert.Equal(TaskPriority.High, created.Priority);
        Assert.Equal(1, await context.Tasks.CountAsync());
    }

    [Fact]
    public async Task GetTaskByIdAsync_ReturnsTask()
    {
        await using var context = CreateContext();
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Existing",
            Status = TaskStatus.Open,
            Priority = TaskPriority.Medium,
            DueDate = DateTime.UtcNow.AddDays(3),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var service = new TaskService(context);
        var result = await service.GetTaskByIdAsync(task.Id);

        Assert.Equal(task.Id, result.Id);
        Assert.Equal("Existing", result.Title);
    }

    [Fact]
    public async Task UpdateTaskAsync_UpdatesTask()
    {
        await using var context = CreateContext();
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Old title",
            Status = TaskStatus.Open,
            Priority = TaskPriority.Low,
            DueDate = DateTime.UtcNow.AddDays(3),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var service = new TaskService(context);
        var updated = await service.UpdateTaskAsync(task.Id, new UpdateTaskDto
        {
            Title = "New title",
            Description = "Updated description",
            Status = TaskStatus.InProgress,
            Priority = TaskPriority.High,
            DueDate = DateTime.UtcNow.AddDays(5)
        });

        Assert.Equal("New title", updated.Title);
        Assert.Equal(TaskStatus.InProgress, updated.Status);
        Assert.Equal(TaskPriority.High, updated.Priority);
        Assert.True(updated.UpdatedAt >= task.UpdatedAt);
    }

    [Fact]
    public async Task DeleteTaskAsync_RemovesTask()
    {
        await using var context = CreateContext();
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Delete me",
            Status = TaskStatus.Open,
            Priority = TaskPriority.Low,
            DueDate = DateTime.UtcNow.AddDays(2),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var service = new TaskService(context);
        await service.DeleteTaskAsync(task.Id);

        Assert.False(await context.Tasks.AnyAsync());
    }

    [Fact]
    public async Task GetTasksAsync_FiltersByStatusAndPriority()
    {
        await using var context = CreateContext();
        context.Tasks.AddRange(
            new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Task 1",
                Status = TaskStatus.Open,
                Priority = TaskPriority.High,
                DueDate = DateTime.UtcNow.AddDays(2),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Task 2",
                Status = TaskStatus.Completed,
                Priority = TaskPriority.High,
                DueDate = DateTime.UtcNow.AddDays(2),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        await context.SaveChangesAsync();

        var service = new TaskService(context);
        var results = await service.GetTasksAsync(new TaskQueryParameters
        {
            Status = TaskStatus.Open,
            Priority = TaskPriority.High,
            SortBy = "title",
            SortOrder = "asc"
        });

        Assert.Single(results);
        Assert.Equal("Task 1", results[0].Title);
    }

    [Fact]
    public async Task GetStatsAsync_ReturnsCounts()
    {
        await using var context = CreateContext();
        context.Tasks.AddRange(
            new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Open",
                Status = TaskStatus.Open,
                Priority = TaskPriority.Low,
                DueDate = DateTime.UtcNow.AddDays(2),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Completed",
                Status = TaskStatus.Completed,
                Priority = TaskPriority.High,
                DueDate = DateTime.UtcNow.AddDays(-1),
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            });
        await context.SaveChangesAsync();

        var service = new TaskService(context);
        var stats = await service.GetStatsAsync();

        Assert.Equal(2, stats.TotalCount);
        Assert.Equal(1, stats.OpenCount);
        Assert.Equal(1, stats.CompletedCount);
        Assert.Equal(1, stats.HighPriorityCount);
    }
}
