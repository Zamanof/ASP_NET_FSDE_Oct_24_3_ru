using ASP_NET_17._TaskFlow_Ownership.Data;
using ASP_NET_17._TaskFlow_Ownership.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ASP_NET_17._TaskFlow_Ownership.Authorization
{
    public class TaskStatusChangeHandler
        : AuthorizationHandler<TaskStatusChangeRequirement, TaskItem>
    {
        private readonly TaskFlowDbContext _context;

        public TaskStatusChangeHandler(TaskFlowDbContext context)
        {
            _context = context;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            TaskStatusChangeRequirement requirement, 
            TaskItem resource)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return;

            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return;
            }

            var project = await _context.Projects
                                .FirstOrDefaultAsync(p => p.Id == resource.ProjectId);

            if (project is null) 
                return;

            if (context.User.IsInRole("Manager") && project.OwnerId == userId)
            {
                context.Succeed(requirement);
                return;
            }

            var isMemeber = await _context.ProjectMembers
                                .AnyAsync(m => m.ProjectId == resource.ProjectId && m.UserId == userId);

            if (isMemeber)
                context.Succeed(requirement);
        }
    }
}
