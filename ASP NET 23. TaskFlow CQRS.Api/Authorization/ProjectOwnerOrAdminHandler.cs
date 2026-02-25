using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using ASP_NET_23._TaskFlow_CQRS.Domain;

namespace ASP_NET_23._TaskFlow_CQRS.Api.Authorization;

public class ProjectOwnerOrAdminHandler : AuthorizationHandler<ProjectOwnerOrAdminRequirement, Project>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ProjectOwnerOrAdminRequirement requirement,
        Project resource)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Task.CompletedTask;
        if (context.User.IsInRole("Admin")) { context.Succeed(requirement); return Task.CompletedTask; }
        if (context.User.IsInRole("Manager") && resource.OwnerId == userId) context.Succeed(requirement);
        return Task.CompletedTask;
    }
}
