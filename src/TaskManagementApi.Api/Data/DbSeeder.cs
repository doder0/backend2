using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Api.Entities;
using TaskManagementApi.Api.Enums;

namespace TaskManagementApi.Api.Data;

public sealed class DbSeeder
{
    private readonly ApplicationDbContext _context;

    public DbSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        if (await _context.Tasks.AnyAsync())
        {
            return;
        }

        var now = DateTime.UtcNow;

        _context.Tasks.AddRange(
            new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Prepare project portfolio",
                Description = "Finish the backend API and document the setup.",
                Status = TaskStatus.InProgress,
                Priority = TaskPriority.High,
                DueDate = now.AddDays(5),
                CreatedAt = now,
                UpdatedAt = now
            },
            new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Review pull requests",
                Description = "Check the pending code review queue.",
                Status = TaskStatus.Open,
                Priority = TaskPriority.Medium,
                DueDate = now.AddDays(2),
                CreatedAt = now,
                UpdatedAt = now
            },
            new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Close completed task tickets",
                Description = "Archive finished work items and update reporting.",
                Status = TaskStatus.Completed,
                Priority = TaskPriority.Low,
                DueDate = now.AddDays(1),
                CreatedAt = now,
                UpdatedAt = now
            });

        await _context.SaveChangesAsync();
    }
}
