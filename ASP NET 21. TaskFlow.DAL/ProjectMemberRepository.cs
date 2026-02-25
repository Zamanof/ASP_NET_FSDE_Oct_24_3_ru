using ASP_NET_21._TaskFlow.Data;
using ASP_NET_21._TaskFlow.Models;
using Microsoft.EntityFrameworkCore;

namespace ASP_NET_21._TaskFlow.DAL;

public class ProjectMemberRepository : IProjectMemberRepository
{
    private readonly TaskFlowDbContext _context;

    public ProjectMemberRepository(TaskFlowDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ProjectMember member)
    {
        _context.ProjectMembers.Add(member);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int projectId, string userId)
    {
        return await _context.ProjectMembers
            .AnyAsync(m => m.ProjectId == projectId && m.UserId == userId);
    }

    public async Task<ProjectMember?> FindAsync(int projectId, string userId)
    {
        return await _context.ProjectMembers
            .FirstOrDefaultAsync(m => m.ProjectId == projectId && m.UserId == userId);
    }

    public async Task<IEnumerable<string>> GetMemberUserIdsAsync(int projectId)
    {
        return await _context.ProjectMembers
            .Where(m => m.ProjectId == projectId)
            .Select(m => m.UserId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProjectMember>> GetByProjectIdWithUserAsync(int projectId)
    {
        return await _context.ProjectMembers
            .Include(m => m.User)
            .Where(m => m.ProjectId == projectId)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task RemoveAsync(ProjectMember member)
    {
        _context.ProjectMembers.Remove(member);
        await _context.SaveChangesAsync();
    }
}
