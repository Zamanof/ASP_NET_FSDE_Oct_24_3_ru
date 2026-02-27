using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ASP_NET_23._TaskFlow_CQRS.Application.Common;
using ASP_NET_23._TaskFlow_CQRS.Application.DTOs;
using ASP_NET_23._TaskFlow_CQRS.Application.Services;
using MediatR;
using ASP_NET_23._TaskFlow_CQRS.Application.Features.Tasks;

namespace ASP_NET_23._TaskFlow_CQRS.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "UserOrAbove")]
public class TaskItemsController : ControllerBase
{
    private readonly ITaskItemService _taskItemService;
    private readonly IProjectService _projectService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IMediator _mediator;

    public TaskItemsController(ITaskItemService taskItemService, IProjectService projectService, IAuthorizationService authorizationService, IMediator mediator)
    {
        _taskItemService = taskItemService;
        _projectService = projectService;
        _authorizationService = authorizationService;
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<TaskItemResponseDto>>> Create([FromBody] CreateTaskItemDto createTaskItem)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResponse<TaskItemResponseDto> { Success = false, Message = "Invalid model state", Data = default });

        var project = await _projectService.GetProjectEntityAsync(createTaskItem.ProjectId);
        if (project == null) return NotFound();
        var authResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");
        if (!authResult.Succeeded) return Forbid();

        try
        {
            var createdTaskItem = await _taskItemService.CreateAsync(createTaskItem);
            return CreatedAtAction(nameof(GetById), new { id = createdTaskItem.Id },
                ApiResponse<TaskItemResponseDto>.SuccessResponse(createdTaskItem, "Task item created successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ApiResponse<TaskItemResponseDto> { Success = false, Message = ex.Message, Data = default });
        }
    }

    [HttpGet("all")]
    public async Task<ActionResult<ApiResponse<IEnumerable<TaskItemResponseDto>>>> GetAll()
    {
        var taskItems = await _taskItemService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<TaskItemResponseDto>>.SuccessResponse(taskItems));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<TaskItemResponseDto>>> GetById(int id)
    {
        var task = await _taskItemService.GetTaskEntityAsync(id);
        if (task == null) return NotFound();
        var project = await _projectService.GetProjectEntityAsync(task.ProjectId);
        if (project == null) return NotFound();
        var authResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectMemberOrHigher");
        if (!authResult.Succeeded) return Forbid();
        var taskItem = await _mediator.Send(new GetTaskByIdQuery(id));
        if (taskItem is null) return NotFound(new ApiResponse<TaskItemResponseDto> { Success = false, Message = $"TaskItem with ID {id} not found", Data = default });
        return Ok(ApiResponse<TaskItemResponseDto>.SuccessResponse(taskItem));
    }

    [HttpGet("project/{projectId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<TaskItemResponseDto>>>> GetByProjectId(int projectId)
    {
        var project = await _projectService.GetProjectEntityAsync(projectId);
        if (project == null) return NotFound();
        var authResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectMemberOrHigher");
        if (!authResult.Succeeded) return Forbid();
        var taskItems = await _taskItemService.GetByProjectIdAsync(projectId);
        return Ok(ApiResponse<IEnumerable<TaskItemResponseDto>>.SuccessResponse(taskItems));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<TaskItemResponseDto>>> Update(int id, [FromBody] UpdateTaskItemDto updateTaskItem)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResponse<TaskItemResponseDto> { Success = false, Message = "Invalid model state", Data = default });

        var task = await _taskItemService.GetTaskEntityAsync(id);
        if (task == null) return NotFound();
        var project = await _projectService.GetProjectEntityAsync(task.ProjectId);
        if (project == null) return NotFound();
        var authResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");
        if (!authResult.Succeeded) return Forbid();
        var updatedTaskItem = await _taskItemService.UpdateAsync(id, updateTaskItem);
        if (updatedTaskItem is null) return NotFound(new ApiResponse<TaskItemResponseDto> { Success = false, Message = $"TaskItem with ID {id} not found", Data = default });
        return Ok(ApiResponse<TaskItemResponseDto>.SuccessResponse(updatedTaskItem, "Task item updated successfully"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var task = await _taskItemService.GetTaskEntityAsync(id);
        if (task == null) return NotFound();
        var project = await _projectService.GetProjectEntityAsync(task.ProjectId);
        if (project == null) return NotFound();
        var authResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");
        if (!authResult.Succeeded) return Forbid();
        var isDeleted = await _taskItemService.DeleteAsync(id);
        if (!isDeleted) return NotFound(new ApiResponse<object> { Success = false, Message = $"TaskItem with ID {id} not found", Data = default });
        return NoContent();
    }

    [HttpPatch("{id}/status")]
    public async Task<ActionResult<ApiResponse<TaskItemResponseDto>>> UpdateStatus(int id, [FromBody] TaskStatusUpdateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResponse<TaskItemResponseDto> { Success = false, Message = "Invalid model state", Data = default });

        var task = await _taskItemService.GetTaskEntityAsync(id);
        if (task == null) return NotFound();
        var authResult = await _authorizationService.AuthorizeAsync(User, task, "TaskStatusChange");
        if (!authResult.Succeeded) return Forbid();
        var updatedTaskItem = await _taskItemService.UpdateStatusAsync(id, request);
        if (updatedTaskItem is null) return NotFound(new ApiResponse<TaskItemResponseDto> { Success = false, Message = $"TaskItem with ID {id} not found", Data = default });
        return Ok(ApiResponse<TaskItemResponseDto>.SuccessResponse(updatedTaskItem, "Task item updated successfully"));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<TaskItemResponseDto>>>> GetPaged([FromQuery] TaskItemQueryParams queryParams)
    {
        var result = await _taskItemService.GetPagedAsync(queryParams);
        return Ok(ApiResponse<PagedResult<TaskItemResponseDto>>.SuccessResponse(result));
    }
}
