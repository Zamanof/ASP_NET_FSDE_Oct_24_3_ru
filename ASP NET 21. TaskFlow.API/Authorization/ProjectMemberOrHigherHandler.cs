using ASP_NET_21._TaskFlow.BLL.Services;
using ASP_NET_21._TaskFlow.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ASP_NET_21._TaskFlow.API.Authorization;

public class ProjectMemberOrHigherHandler
    : AuthorizationHandler<ProjectMemberOrHigherRequirment, Project>
{
    private readonly IProjectService _projectService;

    public ProjectMemberOrHigherHandler(IProjectService projectService)
    {
        _projectService = projectService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ProjectMemberOrHigherRequirment requirement,
        Project resource)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return;

        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return;
        }

        if (context.User.IsInRole("Manager") && resource.OwnerId == userId)
        {
            context.Succeed(requirement);
            return;
        }

        if (await _projectService.IsMemberAsync(resource.Id, userId))
            context.Succeed(requirement);
    }
}
