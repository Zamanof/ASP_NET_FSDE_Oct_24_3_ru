using ASP_NET_21._TaskFlow.BLL.Common;
using ASP_NET_21._TaskFlow.BLL.DTOs;
using ASP_NET_21._TaskFlow.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASP_NET_21._TaskFlow.API.Controllers;

/// <summary>
/// Controller for managing projects.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "UserOrAbove")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly IAuthorizationService _authorizationService;

    private string? UserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier);

    private IList<string> UserRoles =>
        User.Claims.Where(c => c.Type == ClaimTypes.Role)
                   .Select(c => c.Value)
                   .ToList();


    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectsController"/> class.
    /// </summary>
    /// <param name="projectService">Service for project operations.</param>
    /// <param name="authorizationService">Service for authorization checks.</param>
    public ProjectsController(IProjectService projectService, IAuthorizationService authorizationService)
    {
        _projectService = projectService;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Creates a new project.
    /// </summary>
    /// <param name="createProjectDto">The payload used to create the project.</param>
    /// <returns>The created project wrapped in <see cref="ApiResponse{ProjectResponseDto}"/>.</returns>
    /// <response code="201">The project was successfully created.</response>
    /// <response code="400">The request body is invalid.</response>
    [HttpPost]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<ActionResult<ApiResponse<ProjectResponseDto>>> Create([FromBody] CreateProjectDto createProjectDto)
    {
        var ownerId = UserId ?? throw new InvalidOperationException("User ID not found in claims");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdProject = await _projectService.CreateAsync(createProjectDto, ownerId);

        return CreatedAtAction(nameof(GetById), new { id = createdProject.Id },
            ApiResponse<ProjectResponseDto>.SuccessResponse(createdProject, "Project created successfully"));
    }

    /// <summary>
    /// Retrieves a project by its identifier.
    /// </summary>
    /// <param name="id">The project identifier.</param>
    /// <returns>The project details wrapped in <see cref="ApiResponse{ProjectResponseDto}"/>.</returns>
    /// <response code="200">The project was found and returned.</response>
    /// <response code="404">A project with the specified identifier was not found.</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ProjectResponseDto>>> GetById(int id)
    {
        var project = await _projectService.GetProjectEntityAsync(id);

        if (project is null)
            return NotFound($"Project with ID {id} not found");

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectMemberOrHigher");

        if (!authorizationResult.Succeeded)
            return Forbid();

        var dto = await _projectService.GetByIdAsync(id);

        return Ok(ApiResponse<ProjectResponseDto>.SuccessResponse(dto!));
    }

    /// <summary>
    /// Retrieves all projects.
    /// </summary>
    /// <returns>A list of all projects wrapped in"/>.</returns>
    /// <response code="200">Returns the list of projects.</response>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProjectResponseDto>>>> GetAll()
    {
        var userId = UserId ?? throw new InvalidOperationException("User ID not found in claims");

        var projects = await _projectService.GetAllForUserAsync(userId, UserRoles);
        return Ok(ApiResponse<IEnumerable<ProjectResponseDto>>.SuccessResponse(projects));
    }

    /// <summary>
    /// Updates an existing project.
    /// </summary>
    /// <param name="id">The project identifier.</param>
    /// <param name="updateProjectDto">The payload used to update the project.</param>
    /// <returns>The updated project details wrapped in <see cref="ApiResponse{ProjectResponseDto}"/>.</returns>
    /// <response code="200">The project was successfully updated.</response>
    /// <response code="400">The request body is invalid.</response>
    /// <response code="404">A project with the specified identifier was not found.</response>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ProjectResponseDto>>> Update(int id, [FromBody] UpdateProjectDto updateProjectDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existingProject = await _projectService.GetProjectEntityAsync(id);

        if (existingProject is null)
            return NotFound($"Project with ID {id} not found");

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, existingProject, "ProjectOwnerOrAdmin");

        if (!authorizationResult.Succeeded)
            return Forbid();

        var updatedProject = await _projectService.UpdateAsync(id, updateProjectDto);

        return Ok(ApiResponse<ProjectResponseDto>.SuccessResponse(updatedProject!, "Project updated successfully"));
    }

    /// <summary>
    /// Deletes a project by its identifier.
    /// </summary>
    /// <param name="id">The project identifier.</param>
    /// <returns>Result of the delete operation wrapped in</returns>
    /// <response code="200">The project was successfully deleted.</response>
    /// <response code="404">A project with the specified identifier was not found.</response>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {

        var existingProject = await _projectService.GetProjectEntityAsync(id);

        if (existingProject is null)
            return NotFound($"Project with ID {id} not found");

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, existingProject, "ProjectOwnerOrAdmin");

        if (!authorizationResult.Succeeded)
            return Forbid();

        var isDeleted = await _projectService.DeleteAsync(id);

        if (!isDeleted)
            return NotFound($"Project with ID {id} not found");

        return NoContent();
    }

    /// <summary>
    /// Retrieves a list of members for a specific project.
    /// </summary>
    /// <param name="projectId"></param>
    /// <returns></returns>
    [HttpGet("{projectId}/members")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProjectMemberResponse>>>> GetMembers(int projectId)
    {
        var project = await _projectService.GetProjectEntityAsync(projectId);

        if (project is null)
            return NotFound($"Project with ID {projectId} not found");

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectMemberOrHigher");
        if (!authorizationResult.Succeeded)
            return Forbid();

        var members = await _projectService.GetMembersAsync(projectId);

        return Ok(ApiResponse<IEnumerable<ProjectMemberResponse>>.SuccessResponse(members));
    }
    /// <summary>
    /// Retrieves a list of users who are not currently members of the specified project and can be added as members.
    /// </summary>
    /// <param name="projectId"></param>
    /// <returns></returns>
    [HttpGet("{projectId}/available-users")]
    public async Task<ActionResult<ApiResponse<IEnumerable<AvailableUserDto>>>> GetAvailableUsersToAdd(int projectId)
    {
        var project = await _projectService.GetProjectEntityAsync(projectId);

        if (project is null)
            return NotFound($"Project with ID {projectId} not found");

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");

        if (!authorizationResult.Succeeded)
            return Forbid();

        var availableUsers = await _projectService.GetAvailableUsersToAddAsync(projectId);

        return Ok(ApiResponse<IEnumerable<AvailableUserDto>>.SuccessResponse(availableUsers));
    }

    /// <summary>
    /// Adds a member to a project.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="addProjectMemberDto"></param>
    /// <returns></returns>
    [HttpPost("{projectId}/members")]
    public async Task<ActionResult> AddMember(int projectId, [FromBody] AddProjectMemberDto addProjectMemberDto)
    {
        var project = await _projectService.GetProjectEntityAsync(projectId);

        if (project is null)
            return NotFound($"Project with ID {projectId} not found");

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");

        if (!authorizationResult.Succeeded)
            return Forbid();

        var userIdOrEmail = addProjectMemberDto.UserId ?? addProjectMemberDto.Email?.Trim();

        if (string.IsNullOrEmpty(userIdOrEmail))
            return BadRequest("Either UserId or Email must be provided.");

        var isAdded = await _projectService.AddMemberAsync(projectId, userIdOrEmail);

        if (!isAdded)
            return BadRequest("Failed to add member. Please ensure the user exists and is not already a member.");
        return NoContent();
    }
    /// <summary>
    /// Removes a member from a project.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpDelete("{projectId}/members/{userId}")]
    public async Task<ActionResult> RemoveMember(int projectId, string userId)
    {
        var project = await _projectService.GetProjectEntityAsync(projectId);

        if (project is null)
            return NotFound($"Project with ID {projectId} not found");

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");

        if (!authorizationResult.Succeeded)
            return Forbid();

        var isRemoved = await _projectService.RemoveMemberAsync(projectId, userId);

        if (!isRemoved)
            return BadRequest("Failed to remove member. Please ensure the user is a member of the project.");

        return NoContent();
    }
}