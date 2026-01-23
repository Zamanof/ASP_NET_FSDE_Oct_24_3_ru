using ASP_NET_09._TaskFlow_DTOs.Models;
using ASP_NET_09._TaskFlow_DTOs.Services;
using ASP_NET_09._TaskFlow_DTOs.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ASP_NET_09._TaskFlow_DTOs.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TaskItemsController : ControllerBase
{
    private readonly ITaskItemResponseDtoService _TaskItemResponseDtoService;

    public TaskItemsController(ITaskItemResponseDtoService TaskItemResponseDtoService)
    {
        _TaskItemResponseDtoService = TaskItemResponseDtoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskItemResponseDto>>> GetAll()
    {
        var TaskItemResponseDtos = await _TaskItemResponseDtoService.GetAllAsync();
        return Ok(TaskItemResponseDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskItemResponseDto>> GetById(int id)
    {
        var TaskItemResponseDto = await _TaskItemResponseDtoService.GetByIdAsync(id);
        if (TaskItemResponseDto is null)
            return NotFound($"TaskItemResponseDto with ID {id} not found");
        return Ok(TaskItemResponseDto);
    }

    [HttpGet("project/{projectId}")]
    public async Task<ActionResult<TaskItemResponseDto>> GetByProjectId(int projectId)
    {
        var TaskItemResponseDto = await _TaskItemResponseDtoService.GetByIdAsync(projectId);
        if (TaskItemResponseDto is null)
            return NotFound($"TaskItemResponseDto with ID {projectId} not found");
        return Ok(TaskItemResponseDto);
    }

    [HttpPost]
    public async Task<ActionResult<TaskItemResponseDto>> Create([FromBody] TaskItemResponseDto TaskItemResponseDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var createdTaskItemResponseDto = await _TaskItemResponseDtoService.CreateAsync(TaskItemResponseDto);
            return CreatedAtAction(
            nameof(GetById),
            new { id = createdTaskItemResponseDto.Id },
            createdTaskItemResponseDto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex);
        }


        
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TaskItemResponseDto?>> Update(int id, [FromBody] TaskItemResponseDto TaskItemResponseDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var updatedTaskItemResponseDto = await _TaskItemResponseDtoService.UpdateAsync(id, TaskItemResponseDto);

        if (updatedTaskItemResponseDto is null)
            return NotFound($"TaskItemResponseDto with ID {id} not found");

        return Ok(updatedTaskItemResponseDto);
    }

    [HttpDelete("{id}")]
    // base_path/id
    public async Task<ActionResult> Delete(int id)
    {
        var isDeleted = await _TaskItemResponseDtoService.DeleteAsync(id);

        if (!isDeleted)
            return NotFound($"TaskItemResponseDto with ID {id} not found");

        return NoContent();
    }
}
