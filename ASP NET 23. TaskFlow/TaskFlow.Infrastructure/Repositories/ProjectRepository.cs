using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly TaskFlowDbContext _context;

    public ProjectRepository(TaskFlowDbContext context) => _context = context;

    public async Task<Project> AddAsync(Project project)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        await _context.Entry(project).Collection(p => p.Tasks).LoadAsync();
        return project;
    }

    public async Task<Project?> FindAsync(int id) => await _context.Projects.FindAsync([id]);

    public async Task<IEnumerable<Project>?> GetAllForUserAsync(string userId, IList<string> roles)
    {
        var query = _context.Projects.Include(p => p.Tasks).AsQueryable();
        if (roles.Contains("Admin")) { }
        else if (roles.Contains("Manager"))
            query = query.Where(p => p.OwnerId == userId || p.Members.Any(m => m.UserId == userId));
        else
            query = query.Where(p => p.Members.Any(m => m.UserId == userId));
        return await query.ToListAsync();
    }

    public async Task<Project?> GetByIdWithTasksAndMembersAsync(int id) =>
        await _context.Projects.Include(p => p.Tasks).Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Project?> GetByIdWithTasksAsync(int id) =>
        await _context.Projects.Include(p => p.Tasks).FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Project?> GetByTaskIdAsync(int taskId)
    {
        var task = await _context.TaskItems.FirstOrDefaultAsync(t => t.Id == taskId);
        return task is null ? null : await GetByIdWithTasksAndMembersAsync(task.ProjectId);
    }

    public async Task RemoveAsync(Project project)
    {
        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Project project)
    {
        _context.Projects.Update(project);
        await _context.SaveChangesAsync();
    }
}
