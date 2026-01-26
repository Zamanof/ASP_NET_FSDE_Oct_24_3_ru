using ASP_NET_10.__TaskFlow_AutoMapper.Data;
using ASP_NET_10.__TaskFlow_AutoMapper.DTOs.TaskItem_DTOs;
using ASP_NET_10.__TaskFlow_AutoMapper.Models;
using ASP_NET_10.__TaskFlow_AutoMapper.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ASP_NET_10.__TaskFlow_AutoMapper.Services;

public class TaskItemService : ITaskItemService
{

    private readonly TaskFlowDbContext _context;
    private readonly IMapper _mapper;

    public TaskItemService(TaskFlowDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<TaskItemResponseDto> CreateAsync(TaskItemCreateRequest createRequest)
    {
        var isProjectExists =
            await _context
                      .Projects
                         .AnyAsync(p => p.Id == createRequest.ProjectId);

        if (!isProjectExists)
            throw new ArgumentException($"Project with ID {createRequest.ProjectId} not found");

        TaskItem taskItem = _mapper.Map<TaskItem>(createRequest);


        _context.TaskItems.Add(taskItem);
        await _context.SaveChangesAsync();

        await _context
                    .Entry(taskItem)
                    .Reference(t => t.Project)
                    .LoadAsync();

        return _mapper.Map<TaskItemResponseDto>(taskItem);
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

        return _mapper.Map<IEnumerable<TaskItemResponseDto>>(tasks);
    }

    public async Task<TaskItemResponseDto?> GetByIdAsync(int id)
    {
        var task = await _context
                        .TaskItems
                        .Include(t => t.Project)
                        .FirstOrDefaultAsync(t => t.Id == id);

        if (task is null) return null;

        return _mapper.Map<TaskItemResponseDto>(task);
    }

    public async Task<IEnumerable<TaskItemResponseDto>> GetByProjectIdAsync(int projectId)
    {
        var tasks = await _context
                        .TaskItems
                        .Include(t => t.Project)
                        .Where(t => t.ProjectId == projectId)
                        .ToListAsync();

        return _mapper.Map<IEnumerable<TaskItemResponseDto>>(tasks);
    }

    public async Task<TaskItemResponseDto?> UpdateAsync(int id, TaskItemUpdateRequest updateRequest)
    {
        var task = await _context
                            .TaskItems
                            .Include(t => t.Project)
                            .FirstOrDefaultAsync(t => t.Id == id);
        if (task is null) return null;

        _mapper.Map(updateRequest, task);

        await _context.SaveChangesAsync();

        return _mapper.Map<TaskItemResponseDto>(task);
    }
}
