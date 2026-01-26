using ASP_NET_09._TaskFlow_DTO.Data;
using ASP_NET_09._TaskFlow_DTO.DTOs.TaskItem_DTOs;
using ASP_NET_09._TaskFlow_DTO.Models;
using ASP_NET_09._TaskFlow_DTO.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ASP_NET_09._TaskFlow_DTO.Services;

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
                         .AnyAsync(p => p.Id == createRequest.ProjectId);

        if (!isProjectExists)
            throw new ArgumentException($"Project with ID {createRequest.ProjectId} not found");

        TaskItem taskItem = new()
        {
            Title = createRequest.Title,
            Description = createRequest.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null!,
            Status = Models.TaskStatus.ToDo,
            ProjectId = createRequest.ProjectId
        };


        _context.TaskItems.Add(taskItem);
        await _context.SaveChangesAsync();

        await _context
                    .Entry(taskItem)
                    .Reference(t => t.Project)
                    .LoadAsync();

        return MapToResponseDto(taskItem);
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
        var tasks = await _context
                        .TaskItems
                        .Include(t => t.Project)
                        .ToListAsync();
        return tasks.Select(MapToResponseDto);
    }

    public async Task<TaskItemResponseDto?> GetByIdAsync(int id)
    {
        var task = await _context
                        .TaskItems
                        .Include(t => t.Project)
                        .FirstOrDefaultAsync(t => t.Id == id);

        if (task is null) return null;

        return MapToResponseDto(task);
    }

    public async Task<IEnumerable<TaskItemResponseDto>> GetByProjectIdAsync(int projectId)
    {
        var tasks = await _context
                        .TaskItems
                        .Include(t => t.Project)
                        .Where(t => t.ProjectId == projectId)
                        .ToListAsync();
        return tasks.Select(MapToResponseDto);
    }

    public async Task<TaskItemResponseDto?> UpdateAsync(int id, TaskItemUpdateRequest updateRequest)
    {
        var task = await _context
                            .TaskItems
                            .Include(t => t.Project)
                            .FirstOrDefaultAsync(t => t.Id == id);
        if (task is null) return null;

        task.Title = updateRequest.Title;
        task.Description = updateRequest.Description;
        task.Status = updateRequest.Status;
        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToResponseDto(task);
    }

    private TaskItemResponseDto MapToResponseDto(TaskItem taskItem)
    {
        return new TaskItemResponseDto
        {
            Id = taskItem.Id,
            Title = taskItem.Title,
            Description = taskItem.Description,
            ProjectId = taskItem.ProjectId,
            Status = taskItem.Status.ToString(),
            ProjectName = taskItem.Project.Name
        };
    }
}
