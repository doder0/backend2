using Microsoft.AspNetCore.Mvc;
using TaskManagementApi.Api.DTOs;
using TaskManagementApi.Api.Services;

namespace TaskManagementApi.Api.Controllers;

[ApiController]
[Route("api/tasks")]
public sealed class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TaskResponseDto>>> GetTasks([FromQuery] TaskQueryParameters queryParameters, CancellationToken cancellationToken)
    {
        var tasks = await _taskService.GetTasksAsync(queryParameters, cancellationToken);
        return Ok(tasks);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskResponseDto>> GetTaskById(Guid id, CancellationToken cancellationToken)
    {
        var task = await _taskService.GetTaskByIdAsync(id, cancellationToken);
        return Ok(task);
    }

    [HttpPost]
    public async Task<ActionResult<TaskResponseDto>> CreateTask([FromBody] CreateTaskDto dto, CancellationToken cancellationToken)
    {
        var createdTask = await _taskService.CreateTaskAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetTaskById), new { id = createdTask.Id }, createdTask);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TaskResponseDto>> UpdateTask(Guid id, [FromBody] UpdateTaskDto dto, CancellationToken cancellationToken)
    {
        var updatedTask = await _taskService.UpdateTaskAsync(id, dto, cancellationToken);
        return Ok(updatedTask);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTask(Guid id, CancellationToken cancellationToken)
    {
        await _taskService.DeleteTaskAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("stats")]
    public async Task<ActionResult<TaskStatsDto>> GetStats(CancellationToken cancellationToken)
    {
        var stats = await _taskService.GetStatsAsync(cancellationToken);
        return Ok(stats);
    }
}
