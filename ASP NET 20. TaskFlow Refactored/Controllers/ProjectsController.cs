using ASP_NET_20._TaskFlow_Refactored.Common;
using ASP_NET_20._TaskFlow_Refactored.DTOs;
using ASP_NET_20._TaskFlow_Refactored.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASP_NET_20._TaskFlow_Refactored.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "UserOrAbove")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly IAuthorizationService _authorizationService;

    private string? UserId => User
                                .FindFirstValue(ClaimTypes.NameIdentifier);
    private IList<string> Roles => User
                                .Claims
                                .Where(c => c.Type == ClaimTypes.Role)
                                .Select(c => c.Value)
                                .ToList();

    public ProjectsController(IProjectService projectService, IAuthorizationService authorizationService)
    {
        _projectService = projectService;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Retrieves all projects.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProjectResponseDto>>>> GetAll()
    {
        var projects = await _projectService.GetAllForUserAsync(UserId!, Roles);
        return Ok(
            ApiResponse<IEnumerable<ProjectResponseDto>>
                .SuccessResponse(projects, "Projects retrieved successfully")
        );
    }

    /// <summary>
    /// Retrieves a project by its identifier.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ProjectResponseDto>>> GetById(int id)
    {
        var project = await _projectService.GetProjectEntityAsync(id);

        if (project is null)
            return NotFound(
                ApiResponse<ProjectResponseDto>
                    .ErrorResponse($"Project with ID {id} not found"));

        var authorizationResult
            = await _authorizationService.AuthorizeAsync(User, project, "ProjectMemberOrHigher");

        if (!authorizationResult.Succeeded)
            return Forbid();

        var projectDto = await _projectService.GetByIdAsync(id);

        return Ok(
            ApiResponse<ProjectResponseDto>
                .SuccessResponse(projectDto!, "Project found")
        );
    }

    /// <summary>
    /// Creates a new project.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Policy = "AdminOrManager")]
    public async Task<ActionResult<ApiResponse<ProjectResponseDto>>> Create(
        [FromBody] ProjectCreateRequest createRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(
                ApiResponse<ProjectResponseDto>
                    .ErrorResponse("Invalid request data")
            );

        var createdProject = await _projectService.CreateAsync(createRequest, UserId!);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdProject.Id },
            ApiResponse<ProjectResponseDto>
                .SuccessResponse(createdProject, "Project created successfully")
        );
    }

    /// <summary>
    /// Updates an existing project.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ProjectResponseDto>>> Update(
        int id,
        [FromBody] ProjectUpdateRequest updateRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(
                ApiResponse<ProjectResponseDto>
                    .ErrorResponse("Invalid request data")
            );

        var project = await _projectService.GetProjectEntityAsync(id);

        if (project is null)
            return NotFound(
                ApiResponse<ProjectResponseDto>
                    .ErrorResponse($"Project with ID {id} not found"));

        var authorizationResult
            = await _authorizationService.AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");

        if (!authorizationResult.Succeeded)
            return Forbid();

        var updatedProject = await _projectService.UpdateAsync(id, updateRequest);

        if (updatedProject is null)
            return NotFound(
                ApiResponse<ProjectResponseDto>
                    .ErrorResponse($"Project with ID {id} not found")
            );

        return Ok(
            ApiResponse<ProjectResponseDto>
                .SuccessResponse(updatedProject, "Project updated successfully")
        );
    }

    /// <summary>
    /// Deletes a project by its identifier.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var project = await _projectService.GetProjectEntityAsync(id);

        if (project is null)
            return NotFound(
                ApiResponse<ProjectResponseDto>
                    .ErrorResponse($"Project with ID {id} not found"));

        var authorizationResult
            = await _authorizationService.AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");

        if (!authorizationResult.Succeeded)
            return Forbid();

        var isDeleted = await _projectService.DeleteAsync(id);

        if (!isDeleted)
            return NotFound(
                ApiResponse<object>
                    .ErrorResponse($"Project with ID {id} not found")
            );

        return NoContent();
    }

    [HttpGet("{projectId}/members")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProjectMemberResponseDto>>>> GetMembers(int projectId)
    {
        var project = await _projectService.GetProjectEntityAsync(projectId);

        if (project is null)
            return NotFound(
                ApiResponse<IEnumerable<ProjectMemberResponseDto>>
                    .ErrorResponse($"Project with ID {projectId} not found"));

        var authorizationResult
            = await _authorizationService.AuthorizeAsync(User, project, "ProjectMemberOrHigher");

        if (!authorizationResult.Succeeded)
            return Forbid();

        var members = await _projectService.GetProjectMembersAsync(projectId);

        return Ok(
            ApiResponse<IEnumerable<ProjectMemberResponseDto>>
                .SuccessResponse(members, "Project members retrieved successfully")
        );
    }

    [HttpGet("{projectId}/available-users")]
    public async Task<ActionResult<ApiResponse<IEnumerable<AvailableUserDto>>>> GetAvailableUsersToAdd(int projectId)
    {
        var project = await _projectService.GetProjectEntityAsync(projectId);

        if (project is null)
            return NotFound(
                ApiResponse<IEnumerable<AvailableUserDto>>
                    .ErrorResponse($"Project with ID {projectId} not found"));

        var authorizationResult
            = await _authorizationService.AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");

        if (!authorizationResult.Succeeded)
            return Forbid();

        var users = await _projectService.GetAvailableUsersToAddAsync(projectId);

        return Ok(
            ApiResponse<IEnumerable<AvailableUserDto>>
                .SuccessResponse(users, "Available users retrieved successfully")
        );
    }

    [HttpPost("{projectId}/members")]
    public async Task<IActionResult> AddMember(int projectId, [FromBody] AddProjectMemberDto request)
    {
        var project = await _projectService.GetProjectEntityAsync(projectId);

        if (project is null)
            return NotFound(
                ApiResponse<object>
                    .ErrorResponse($"Project with ID {projectId} not found"));

        var authorizationResult
            = await _authorizationService.AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");

        if (!authorizationResult.Succeeded)
            return Forbid();

        var userIdOrEmail = request.UserId ?? request.Email?.Trim();

        if (string.IsNullOrEmpty(userIdOrEmail))
            return BadRequest(
                ApiResponse<object>
                    .ErrorResponse("UserId or Email must be provided")
            );

        var isAdded = await _projectService.AddMemberAsync(projectId, userIdOrEmail);

        if (!isAdded)
            return BadRequest(
                ApiResponse<object>
                    .ErrorResponse($"Failed to add member.")
            );
        return NoContent();
    }

    [HttpDelete("{projectId}/members/{userId}")]
    public async Task<IActionResult> RemoveMember(int projectId, string userId)
    {
        var project = await _projectService.GetProjectEntityAsync(projectId);

        if (project is null)
            return NotFound(
                ApiResponse<object>
                    .ErrorResponse($"Project with ID {projectId} not found"));

        var authorizationResult
            = await _authorizationService.AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");

        if (!authorizationResult.Succeeded)
            return Forbid();

        var isRemoved = await _projectService.RemoveMemberAsync(projectId, userId);
        if (!isRemoved)
            return BadRequest(
                ApiResponse<object>
                    .ErrorResponse($"Failed to remove member.")
            );

        return NoContent();
    }
}
