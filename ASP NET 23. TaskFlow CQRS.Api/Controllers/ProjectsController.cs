using ASP_NET_23._TaskFlow_CQRS.Application.Common;
using ASP_NET_23._TaskFlow_CQRS.Application.DTOs;
using ASP_NET_23._TaskFlow_CQRS.Application.Features.Projects.Queries;
using ASP_NET_23._TaskFlow_CQRS.Application.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASP_NET_23._TaskFlow_CQRS.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "UserOrAbove")]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IProjectService _projectService;
    private readonly IAuthorizationService _authorizationService;

    private string? UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);
    private IList<string> UserRoles => User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

    public ProjectsController(IProjectService projectService, IAuthorizationService authorizationService, IMediator mediator)
    {
        _projectService = projectService;
        _authorizationService = authorizationService;
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = "AdminOrManager")]
    public async Task<ActionResult<ApiResponse<ProjectResponseDto>>> Create([FromBody] CreateProjectDto createProjectDto)
    {
        var ownerId = UserId ?? throw new InvalidOperationException("User ID not found in claims");
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var createdProject = await _projectService.CreateAsync(createProjectDto, ownerId);
        return CreatedAtAction(nameof(GetById), new { id = createdProject.Id },
            ApiResponse<ProjectResponseDto>.SuccessResponse(createdProject, "Project created successfully"));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ProjectResponseDto>>> GetById(int id)
    {
        var project = await _projectService.GetProjectEntityAsync(id);
        if (project is null) return NotFound($"Project with ID {id} not found");
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectMemberOrHigher");
        if (!authorizationResult.Succeeded) return Forbid();
        var dto = await _mediator.Send(new GetProjectByIdQuery(id));
        return Ok(ApiResponse<ProjectResponseDto>.SuccessResponse(dto!));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProjectResponseDto>>>> GetAll()
    {
        var userId = UserId ?? throw new InvalidOperationException("User ID not found in claims");
        var projects = await _projectService.GetAllForUserAsync(userId, UserRoles);
        return Ok(ApiResponse<IEnumerable<ProjectResponseDto>>.SuccessResponse(projects));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ProjectResponseDto>>> Update(int id, [FromBody] UpdateProjectDto updateProjectDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var existingProject = await _projectService.GetProjectEntityAsync(id);
        if (existingProject is null) return NotFound($"Project with ID {id} not found");
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, existingProject, "ProjectOwnerOrAdmin");
        if (!authorizationResult.Succeeded) return Forbid();
        var updatedProject = await _projectService.UpdateAsync(id, updateProjectDto);
        return Ok(ApiResponse<ProjectResponseDto>.SuccessResponse(updatedProject!, "Project updated successfully"));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        var existingProject = await _projectService.GetProjectEntityAsync(id);
        if (existingProject is null) return NotFound($"Project with ID {id} not found");
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, existingProject, "ProjectOwnerOrAdmin");
        if (!authorizationResult.Succeeded) return Forbid();
        var isDeleted = await _projectService.DeleteAsync(id);
        if (!isDeleted) return NotFound($"Project with ID {id} not found");
        return NoContent();
    }

    [HttpGet("{projectId}/members")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProjectMemberResponse>>>> GetMembers(int projectId)
    {
        var project = await _projectService.GetProjectEntityAsync(projectId);
        if (project is null) return NotFound($"Project with ID {projectId} not found");
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectMemberOrHigher");
        if (!authorizationResult.Succeeded) return Forbid();
        var members = await _projectService.GetMembersAsync(projectId);
        return Ok(ApiResponse<IEnumerable<ProjectMemberResponse>>.SuccessResponse(members));
    }

    [HttpGet("{projectId}/available-users")]
    public async Task<ActionResult<ApiResponse<IEnumerable<AvailableUserDto>>>> GetAvailableUsersToAdd(int projectId)
    {
        var project = await _projectService.GetProjectEntityAsync(projectId);
        if (project is null) return NotFound($"Project with ID {projectId} not found");
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");
        if (!authorizationResult.Succeeded) return Forbid();
        var availableUsers = await _projectService.GetAvailableUsersToAddAsync(projectId);
        return Ok(ApiResponse<IEnumerable<AvailableUserDto>>.SuccessResponse(availableUsers));
    }

    [HttpPost("{projectId}/members")]
    public async Task<ActionResult> AddMember(int projectId, [FromBody] AddProjectMemberDto addProjectMemberDto)
    {
        var project = await _projectService.GetProjectEntityAsync(projectId);
        if (project is null) return NotFound($"Project with ID {projectId} not found");
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");
        if (!authorizationResult.Succeeded) return Forbid();
        var userIdOrEmail = addProjectMemberDto.UserId ?? addProjectMemberDto.Email?.Trim();
        if (string.IsNullOrEmpty(userIdOrEmail)) return BadRequest("Either UserId or Email must be provided.");
        var isAdded = await _projectService.AddMemberAsync(projectId, userIdOrEmail);
        if (!isAdded) return BadRequest("Failed to add member. Please ensure the user exists and is not already a member.");
        return NoContent();
    }

    [HttpDelete("{projectId}/members/{userId}")]
    public async Task<ActionResult> RemoveMember(int projectId, string userId)
    {
        var project = await _projectService.GetProjectEntityAsync(projectId);
        if (project is null) return NotFound($"Project with ID {projectId} not found");
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");
        if (!authorizationResult.Succeeded) return Forbid();
        var isRemoved = await _projectService.RemoveMemberAsync(projectId, userId);
        if (!isRemoved) return BadRequest("Failed to remove member. Please ensure the user is a member of the project.");
        return NoContent();
    }
}
