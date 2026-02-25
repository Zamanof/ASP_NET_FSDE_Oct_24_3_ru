using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using ASP_NET_23._TaskFlow_CQRS.Application.Services;
using ASP_NET_23._TaskFlow_CQRS.Domain;

namespace ASP_NET_23._TaskFlow_CQRS.Api.Authorization;

public class ProjectMemberOrHigherHandler : AuthorizationHandler<ProjectMemberOrHigherRequirement, Project>
{
    private readonly IProjectService _projectService;

    public ProjectMemberOrHigherHandler(IProjectService projectService) => _projectService = projectService;

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ProjectMemberOrHigherRequirement requirement,
        Project resource)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return;

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
