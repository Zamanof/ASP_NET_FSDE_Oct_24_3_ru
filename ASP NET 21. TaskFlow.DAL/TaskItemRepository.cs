using ASP_NET_21._TaskFlow.Data;
using ASP_NET_21._TaskFlow.Models;
using Microsoft.EntityFrameworkCore;
using TaskStatus = ASP_NET_21._TaskFlow.Models.TaskStatus;

namespace ASP_NET_21._TaskFlow.DAL;

public class TaskItemRepository : ITaskItemRepository
{
    private readonly TaskFlowDbContext _context;

    public TaskItemRepository(TaskFlowDbContext context)
    {
        _context = context;
    }

    public async Task<TaskItem> AddAsync(TaskItem taskItem)
    {
        _context.TaskItems.Add(taskItem);
        await _context.SaveChangesAsync();
        await _context.Entry(taskItem).Reference(t => t.Project).LoadAsync();
        return taskItem;
    }

    public async Task<TaskItem?> FindAsync(int id)
    {
        return await _context.TaskItems.FindAsync(id);
    }

    public async Task<TaskItem?> GetByIdWithProjectAsync(int id)
    {
        return await _context.TaskItems
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<TaskItem>> GetAllWithProjectAsync()
    {
        return await _context.TaskItems
            .Include(t => t.Project)
            .ToListAsync();
    }

    public async Task<IEnumerable<TaskItem>> GetByProjectIdAsync(int projectId)
    {
        return await _context.TaskItems
            .Include(t => t.Project)
            .Where(t => t.ProjectId == projectId)
            .ToListAsync();
    }

    public async Task<bool> ProjectExistsAsync(int projectId)
    {
        return await _context.Projects.AnyAsync(p => p.Id == projectId);
    }

    public async Task<(IEnumerable<TaskItem> Items, int TotalCount)> GetPagedAsync(
        int? projectId,
        string? status,
        string? priority,
        string? search,
        string? sort,
        string? sortDirection,
        int page,
        int size)
    {
        var query = _context.TaskItems.Include(t => t.Project).AsQueryable();

        if (projectId.HasValue)
            query = query.Where(t => t.ProjectId == projectId.Value);

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<TaskStatus>(status, out var statusVal))
            query = query.Where(t => t.Status == statusVal);

        if (!string.IsNullOrWhiteSpace(priority) && Enum.TryParse<TaskPriority>(priority, out var priorityVal))
            query = query.Where(t => t.Priority == priorityVal);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.ToLower();
            query = query.Where(t =>
                t.Title.ToLower().Contains(term) ||
                (t.Description != null && t.Description.ToLower().Contains(term)));
        }

        var totalCount = await query.CountAsync();

        query = ApplySorting(query, sort, sortDirection);

        var items = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task RemoveAsync(TaskItem taskItem)
    {
        _context.TaskItems.Remove(taskItem);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TaskItem taskItem)
    {
        _context.TaskItems.Update(taskItem);
        await _context.SaveChangesAsync();
    }

    private static IQueryable<TaskItem> ApplySorting(
        IQueryable<TaskItem> query,
        string? sort,
        string? sortDirection)
    {
        var isDesc = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return (sort?.ToLower()) switch
        {
            "title" => isDesc ? query.OrderByDescending(t => t.Title) : query.OrderBy(t => t.Title),
            "createdat" => isDesc ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt),
            "status" => isDesc ? query.OrderByDescending(t => t.Status) : query.OrderBy(t => t.Status),
            "priority" => isDesc ? query.OrderByDescending(t => t.Priority) : query.OrderBy(t => t.Priority),
            _ => query.OrderByDescending(t => t.CreatedAt)
        };
    }
}
