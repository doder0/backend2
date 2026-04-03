using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Api.Entities;

namespace TaskManagementApi.Api.Data;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("Tasks");
            entity.HasKey(task => task.Id);
            entity.Property(task => task.Title).IsRequired().HasMaxLength(100);
            entity.Property(task => task.Description).HasMaxLength(1000);
            entity.Property(task => task.Status).IsRequired();
            entity.Property(task => task.Priority).IsRequired();
            entity.Property(task => task.DueDate).IsRequired();
            entity.Property(task => task.CreatedAt).IsRequired();
            entity.Property(task => task.UpdatedAt).IsRequired();
        });
    }
}
