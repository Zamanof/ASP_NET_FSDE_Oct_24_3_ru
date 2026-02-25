using ASP_NET_21._TaskFlow.BLL.Common;
using ASP_NET_21._TaskFlow.BLL.DTOs;
using ASP_NET_21._TaskFlow.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASP_NET_21._TaskFlow.API.Controllers;

/// <summary>
/// Controller for managing task items.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize(Policy ="UserOrAbove")]
public class TaskItemsController : ControllerBase
{
    private readonly ITaskItemService _taskItemService;
    private readonly IProjectService _projectService;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskItemsController"/> class.
    /// </summary>
    /// <param name="taskItemService">Service for task item operations.</param>
    /// <param name="projectService"></param>
    /// <param name="authorizationService"></param>
    public TaskItemsController(ITaskItemService taskItemService, IProjectService projectService, IAuthorizationService authorizationService)
    {
        _taskItemService = taskItemService;
        _projectService = projectService;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Creates a new task item.
    /// </summary>
    /// <param name="createTaskItem">The payload used to create the task item.</param>
    /// <returns>The created task item wrapped in <see cref="TaskItemResponseDto"/>.</returns>
    /// <response code="201">The task item was successfully created.</response>
    /// <response code="400">The request body is invalid.</response>
    [HttpPost]
    //[Tags("create")]
    public async Task<ActionResult<ApiResponse<TaskItemResponseDto>>> Create([FromBody] CreateTaskItemDto createTaskItem)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResponse<TaskItemResponseDto>
            {
                Success = false,
                Message = "Invalid model state",
                Data = default
            });

        var project = await _projectService.GetProjectEntityAsync(createTaskItem.ProjectId);

        if (project == null)
            return NotFound();

        var authResult = await _authorizationService
                                .AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");

        if (!authResult.Succeeded)
            return Forbid();

        try
        {
            var createdTaskItem = await _taskItemService.CreateAsync(createTaskItem);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdTaskItem.Id },
                ApiResponse<TaskItemResponseDto>.SuccessResponse(createdTaskItem, "Task item created successfully")
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ApiResponse<TaskItemResponseDto>
            {
                Success = false,
                Message = ex.Message,
                Data = default
            });
        }
    }

    /// <summary>
    /// Retrieves all task items.
    /// </summary>
    /// <returns>A list of all task items.</returns>
    /// <response code="200">Returns the list of task items.</response>
    [HttpGet("all")]
    //[Tags("tasks get all")]
    public async Task<ActionResult<ApiResponse<IEnumerable<TaskItemResponseDto>>>> GetAll()
    {
        var taskItems = await _taskItemService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<TaskItemResponseDto>>.SuccessResponse(taskItems));
    }

    /// <summary>
    /// Retrieves a task item by its identifier.
    /// </summary>
    /// <param name="id">The task item identifier.</param>
    /// <returns>The task item details.</returns>
    /// <response code="200">The task item was found and returned.</response>
    /// <response code="404">A task item with the specified identifier was not found.</response>
    [HttpGet("{id}")]
    //[Tags("get by id")]
    public async Task<ActionResult<ApiResponse<TaskItemResponseDto>>> GetById(int id)
    {
        var task =await _taskItemService.GetTaskEntityAsync(id);

        if (task == null)
            return NotFound();

        var project = await _projectService.GetProjectEntityAsync(task.ProjectId);

        if (project == null)
            return NotFound();

        var authResult = await _authorizationService
                                .AuthorizeAsync(User, project, "ProjectMemberOrHigher");

        if (!authResult.Succeeded)
            return Forbid();

        var taskItem = await _taskItemService.GetByIdAsync(id);
        if (taskItem is null)
            return NotFound(new ApiResponse<TaskItemResponseDto>
            {
                Success = false,
                Message = $"TaskItem with ID {id} not found",
                Data = default
            });

        return Ok(ApiResponse<TaskItemResponseDto>.SuccessResponse(taskItem));
    }

    /// <summary>
    /// Retrieves all task items for a specific project.
    /// </summary>
    /// <param name="projectId">The project identifier.</param>
    /// <returns>A list of task items for the specified project.</returns>
    /// <response code="200">Returns the list of task items for the project.</response>
    [HttpGet("project/{projectId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<TaskItemResponseDto>>>> 
        GetByProjectId(int projectId)
    {
        var project = await _projectService.GetProjectEntityAsync(projectId);

        if (project == null)
            return NotFound();

        var authResult = await _authorizationService
                                .AuthorizeAsync(User, project, "ProjectMemberOrHigher");

        if (!authResult.Succeeded)
            return Forbid();

        var taskItems = await _taskItemService.GetByProjectIdAsync(projectId);
        return Ok(ApiResponse<IEnumerable<TaskItemResponseDto>>.SuccessResponse(taskItems));
    }

    /// <summary>
    /// Updates an existing task item.
    /// </summary>
    /// <param name="id">The task item identifier.</param>
    /// <param name="updateTaskItem">The payload used to update the task item.</param>
    /// <returns>The updated task item details.</returns>
    /// <response code="200">The task item was successfully updated.</response>
    /// <response code="400">The request body is invalid.</response>
    /// <response code="404">A task item with the specified identifier was not found.</response>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<TaskItemResponseDto>>> Update(int id, [FromBody] UpdateTaskItemDto updateTaskItem)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResponse<TaskItemResponseDto>
            {
                Success = false,
                Message = "Invalid model state",
                Data = default
            });

        var task = await _taskItemService.GetTaskEntityAsync(id);
        if (task == null)
            return NotFound();

        var project = await _projectService.GetProjectEntityAsync(task.ProjectId);

        if (project == null)
            return NotFound();

        var authResult = await _authorizationService
                                .AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");

        if (!authResult.Succeeded)
            return Forbid();

        var updatedTaskItem = await _taskItemService.UpdateAsync(id, updateTaskItem);

        if (updatedTaskItem is null)
            return NotFound(new ApiResponse<TaskItemResponseDto>
            {
                Success = false,
                Message = $"TaskItem with ID {id} not found",
                Data = default
            });

        return Ok(ApiResponse<TaskItemResponseDto>.SuccessResponse(updatedTaskItem, "Task item updated successfully"));
    }

    /// <summary>
    /// Deletes a task item by its identifier.
    /// </summary>
    /// <param name="id">The task item identifier.</param>
    /// <returns>No content if the task item was deleted.</returns>
    /// <response code="200">The task item was successfully deleted.</response>
    /// <response code="404">A task item with the specified identifier was not found.</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {

        var task = await _taskItemService.GetTaskEntityAsync(id);
        if (task == null)
            return NotFound();

        var project = await _projectService.GetProjectEntityAsync(task.ProjectId);

        if (project == null)
            return NotFound();

        var authResult = await _authorizationService
                                .AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");

        if (!authResult.Succeeded)
            return Forbid();
        var isDeleted = await _taskItemService.DeleteAsync(id);

        if (!isDeleted)
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = $"TaskItem with ID {id} not found",
                Data = default
            });

        return NoContent();
    }


    [HttpPatch("{id}/status")]
    public async Task<ActionResult<ApiResponse<TaskItemResponseDto>>> 
        UpdateStatus(int id, [FromBody] TaskStatusUpdateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResponse<TaskItemResponseDto>
            {
                Success = false,
                Message = "Invalid model state",
                Data = default
            });

        var task = await _taskItemService.GetTaskEntityAsync(id);
        if (task == null)
            return NotFound();

        var authResult = await _authorizationService
                                .AuthorizeAsync(User, task, "TaskStatusChange");

        if (!authResult.Succeeded)
            return Forbid();

        var updatedTaskItem = await _taskItemService.UpdateStatusAsync(id, request);

        if (updatedTaskItem is null)
            return NotFound(new ApiResponse<TaskItemResponseDto>
            {
                Success = false,
                Message = $"TaskItem with ID {id} not found",
                Data = default
            });

        return Ok(ApiResponse<TaskItemResponseDto>.SuccessResponse(updatedTaskItem, "Task item updated successfully"));
    }


    /// <summary>
    /// Retrieves a paged, filtered, and sorted list of task items.
    /// </summary>
    /// <param name="queryParams">
    /// Query parameters for pagination, filtering, searching, and sorting task items.
    /// </param>
    /// <returns>
    /// A paged result containing task items that match the specified criteria, wrapped in .
    /// </returns>
    /// <response code="200">Returns the paged list of task items.</response>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<TaskItemResponseDto>>>> GetPaged(
        [FromQuery] TaskItemQueryParams queryParams)
    {
        var result = await _taskItemService.GetPagedAsync(queryParams);
        return Ok(ApiResponse<PagedResult<TaskItemResponseDto>>.SuccessResponse(result));
    }
}
