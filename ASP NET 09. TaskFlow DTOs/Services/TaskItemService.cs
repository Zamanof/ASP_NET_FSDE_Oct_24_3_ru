using ASP_NET_09._TaskFlow_DTOs.Data;
using ASP_NET_09._TaskFlow_DTOs.DTOs.TaskItemResponseDto_DTOs;
using ASP_NET_09._TaskFlow_DTOs.Models;
using ASP_NET_09._TaskFlow_DTOs.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ASP_NET_09._TaskFlow_DTOs.Services;

public class TaskItemService : ITaskItemService
{
    private readonly TaskFlowDbContext _context;

    public TaskItemService(TaskFlowDbContext context)
    {
        _context = context;
    }

    public async Task<TaskItemResponseDto> CreateAsync(TaskItemCreateRequest createRequest)
    {
        var isProjectExists = 
            await _context
                      .Projects
                         .AnyAsync(p=>p.Id == createRequest.ProjectId);

        if (!isProjectExists)
            throw new ArgumentException($"Project with ID {createRequest.ProjectId} not found");

        TaskItem taskItem = new()
        {
            Title = createRequest.Title,
            Description = createRequest.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null!,
            Status = Models.TaskStatus.ToDo,
        };
       

        _context.TaskItems.Add(taskItem);
        await _context.SaveChangesAsync();

        await _context
                    .Entry(taskItem)
                    .Reference(t => t.Project)
                    .LoadAsync();

        return ;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var task = await _context.TaskItems.FindAsync(id);

        if (task is null) return false;

        _context.TaskItems.Remove(task);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<TaskItemResponseDto>> GetAllAsync()
    {
        return await _context
                        .TaskItems
                        .Include(t=> t.Project)
                        .ToListAsync();
    }

    public async Task<TaskItemResponseDto?> GetByIdAsync(int id)
    {
        return await _context
                        .TaskItems
                        .Include(t => t.Project)
                        .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<TaskItemResponseDto>> GetByProjectIdAsync(int projectId)
    {
        return await _context
                        .TaskItems
                        .Include(t => t.Project)
                        .Where(t => t.ProjectId == projectId)
                        .ToListAsync();
    }

    public async Task<TaskItemResponseDto?> UpdateAsync(int id, TaskItemUpdateRequest updateRequest)
    {
        var task = await _context
                            .TaskItems
                            .Include(t=> t.Project)
                            .FirstOrDefaultAsync(t=> t.Id == id);
        if (task is null) return null;

        task.Title = updateRequest.Title;
        task.Description = updateRequest.Description;
        task.Status = updateRequest.Status;
        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return task;
    }
}
