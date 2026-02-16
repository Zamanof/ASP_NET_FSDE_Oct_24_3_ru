using ASP_NET_17._TaskFlow_Ownership.Data;
using ASP_NET_17._TaskFlow_Ownership.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ASP_NET_17._TaskFlow_Ownership.Authorization;

public class ProjectMemberOrHigherHandler
    : AuthorizationHandler<ProjectMemberOrHigherRequirment, Project>
{
    private readonly TaskFlowDbContext _context;

    public ProjectMemberOrHigherHandler(TaskFlowDbContext context)
    {
        _context = context;
    }

    protected async override Task HandleRequirementAsync(
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

        var isMemeber = await _context.ProjectMembers
                                .AnyAsync(m => m.ProjectId == resource.Id && m.UserId == userId);
        
        if (isMemeber)
            context.Succeed(requirement);
    }
}
