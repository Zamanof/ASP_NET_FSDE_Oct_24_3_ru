using AutoMapper;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain;

namespace TaskFlow.Application.Services;

public class TaskItemService : ITaskItemService
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly IMapper _mapper;

    public TaskItemService(ITaskItemRepository taskItemRepository, IMapper mapper)
    {
        _taskItemRepository = taskItemRepository;
        _mapper = mapper;
    }

    public async Task<TaskItemResponseDto> CreateAsync(CreateTaskItemDto createTaskItem)
    {
        if (!await _taskItemRepository.ProjectExistsAsync(createTaskItem.ProjectId))
            throw new ArgumentException($"Project with ID {createTaskItem.ProjectId} not found");

        var taskItem = _mapper.Map<TaskItem>(createTaskItem);
        var added = await _taskItemRepository.AddAsync(taskItem);
        return _mapper.Map<TaskItemResponseDto>(added);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var task = await _taskItemRepository.FindAsync(id);
        if (task is null) return false;
        await _taskItemRepository.RemoveAsync(task);
        return true;
    }

    public async Task<IEnumerable<TaskItemResponseDto>> GetAllAsync()
    {
        var tasks = await _taskItemRepository.GetAllWithProjectAsync();
        return _mapper.Map<IEnumerable<TaskItemResponseDto>>(tasks);
    }

    public async Task<TaskItemResponseDto?> GetByIdAsync(int id)
    {
        var task = await _taskItemRepository.GetByIdWithProjectAsync(id);
        return _mapper.Map<TaskItemResponseDto?>(task);
    }

    public async Task<IEnumerable<TaskItemResponseDto>> GetByProjectIdAsync(int projectId)
    {
        var tasks = await _taskItemRepository.GetByProjectIdAsync(projectId);
        return _mapper.Map<IEnumerable<TaskItemResponseDto>>(tasks);
    }

    public async Task<TaskItemResponseDto?> UpdateAsync(int id, UpdateTaskItemDto updateTaskItem)
    {
        var task = await _taskItemRepository.GetByIdWithProjectAsync(id);
        if (task is null) return null;
        _mapper.Map(updateTaskItem, task);
        await _taskItemRepository.UpdateAsync(task);
        return _mapper.Map<TaskItemResponseDto>(task);
    }

    public async Task<PagedResult<TaskItemResponseDto>> GetPagedAsync(TaskItemQueryParams queryParams)
    {
        queryParams.Validate();
        var (items, totalCount) = await _taskItemRepository.GetPagedAsync(
            queryParams.ProjectId,
            queryParams.Status,
            queryParams.Priority,
            queryParams.Search,
            queryParams.Sort,
            queryParams.SortDirection,
            queryParams.Page,
            queryParams.Size);
        var taskDtos = _mapper.Map<IEnumerable<TaskItemResponseDto>>(items);
        return PagedResult<TaskItemResponseDto>.Create(taskDtos, queryParams.Page, queryParams.Size, totalCount);
    }

    public async Task<TaskItemResponseDto?> UpdateStatusAsync(int id, TaskStatusUpdateRequest request)
    {
        var task = await _taskItemRepository.GetByIdWithProjectAsync(id);
        if (task is null) return null;
        task.Status = request.Status;
        task.UpdatedAt = DateTime.UtcNow;
        await _taskItemRepository.UpdateAsync(task);
        return _mapper.Map<TaskItemResponseDto?>(task);
    }

    public async Task<TaskItem?> GetTaskEntityAsync(int id) =>
        await _taskItemRepository.GetByIdWithProjectAsync(id);
}
