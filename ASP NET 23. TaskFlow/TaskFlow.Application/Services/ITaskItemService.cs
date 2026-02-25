using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs;
using TaskFlow.Domain;

namespace TaskFlow.Application.Services;

public interface ITaskItemService
{
    Task<IEnumerable<TaskItemResponseDto>> GetAllAsync();
    Task<IEnumerable<TaskItemResponseDto>> GetByProjectIdAsync(int projectId);
    Task<TaskItem?> GetTaskEntityAsync(int id);
    Task<TaskItemResponseDto?> GetByIdAsync(int id);
    Task<PagedResult<TaskItemResponseDto>> GetPagedAsync(TaskItemQueryParams queryParams);
    Task<TaskItemResponseDto> CreateAsync(CreateTaskItemDto createTaskItem);
    Task<TaskItemResponseDto?> UpdateAsync(int id, UpdateTaskItemDto updateTaskItem);
    Task<TaskItemResponseDto?> UpdateStatusAsync(int id, TaskStatusUpdateRequest request);
    Task<bool> DeleteAsync(int id);
}
