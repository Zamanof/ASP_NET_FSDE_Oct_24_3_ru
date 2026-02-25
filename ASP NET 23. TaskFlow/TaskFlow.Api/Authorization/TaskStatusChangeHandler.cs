using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using TaskFlow.Application.Services;
using TaskFlow.Domain;

namespace TaskFlow.Api.Authorization;

public class TaskStatusChangeHandler : AuthorizationHandler<TaskStatusChangeRequirement, TaskItem>
{
    private readonly IProjectService _projectService;

    public TaskStatusChangeHandler(IProjectService projectService) => _projectService = projectService;

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        TaskStatusChangeRequirement requirement,
        TaskItem resource)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return;

        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return;
        }

        var project = await _projectService.GetProjectEntityAsync(resource.ProjectId);
        if (project is null) return;

        if (context.User.IsInRole("Manager") && project.OwnerId == userId)
        {
            context.Succeed(requirement);
            return;
        }

        if (await _projectService.IsMemberAsync(resource.ProjectId, userId))
            context.Succeed(requirement);
    }
}
