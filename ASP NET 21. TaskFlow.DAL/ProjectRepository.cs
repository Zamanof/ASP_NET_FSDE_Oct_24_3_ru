using ASP_NET_21._TaskFlow.Data;
using ASP_NET_21._TaskFlow.Models;
using Microsoft.EntityFrameworkCore;

namespace ASP_NET_21._TaskFlow.DAL;

public class ProjectRepository : IProjectRepository
{
    private readonly TaskFlowDbContext _context;

    public ProjectRepository(TaskFlowDbContext context)
    {
        _context = context;
    }

    public async Task<Project> AddAsync(Project project)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        await _context.Entry(project).Collection(p=>p.Tasks).LoadAsync();
        return project;
    }

    public async Task<Project?> FindAsync(int id)
    {
        return await _context.Projects.FindAsync([id]);
    }

    public async Task<IEnumerable<Project>?> GetAllForUserAsync(string userId, IList<string> roles)
    {
        IQueryable<Project> query = _context.Projects
                                        .Include(p => p.Tasks);

        if (roles.Contains("Admin")) { }
        else if (roles.Contains("Manager"))
        {
            query = query.Where(p => p.OwnerId == userId || p.Members.Any(m => m.UserId == userId));
        }
        else
        {
            query = query.Where(p => p.Members.Any(m => m.UserId == userId));
        }

       return await query.ToListAsync();
    }


    public async Task<Project?> GetByIdWithTasksAndMembersAsync(int id)
    {
        return await _context.Projects
             .Include(p => p.Tasks)
             .Include(p => p.Members)
             .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Project?> GetByIdWithTasksAsync(int id)
    {
        return await _context.Projects
             .Include(p => p.Tasks)
             .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Project?> GetByTaskIdAsync(int taskId)
    {
        var task = await _context.TaskItems
           .FirstOrDefaultAsync(p => p.Id == taskId);
        if (task == null)
            return null;
        return await GetByIdWithTasksAndMembersAsync(task.ProjectId);
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
