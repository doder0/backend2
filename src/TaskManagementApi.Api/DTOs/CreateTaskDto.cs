using System.ComponentModel.DataAnnotations;
using TaskManagementApi.Api.Enums;

namespace TaskManagementApi.Api.DTOs;

public sealed class CreateTaskDto : IValidatableObject
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    public TaskStatus Status { get; set; }

    [Required]
    public TaskPriority Priority { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DueDate <= DateTime.UtcNow)
        {
            yield return new ValidationResult("DueDate must be in the future.", new[] { nameof(DueDate) });
        }
    }
}
