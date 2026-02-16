using ASP_NET_17._TaskFlow_Ownership.Common;
using ASP_NET_17._TaskFlow_Ownership.DTOs.TaskItem_DTOs;
using ASP_NET_17._TaskFlow_Ownership.Models;

namespace ASP_NET_17._TaskFlow_Ownership.Services.Interfaces
{
    public interface ITaskItemService
    {
        Task<IEnumerable<TaskItemResponseDto>> GetAllAsync();
        Task<PagedResult<TaskItemResponseDto>> GetPagedAsync(TaskItemQueryParams queryParams);
        Task<IEnumerable<TaskItemResponseDto>> GetByProjectIdAsync(int projectId);
        Task<TaskItemResponseDto?> GetByIdAsync(int id);
        Task<TaskItem?> GetTaskEntityAsync(int id);
        Task<TaskItemResponseDto> CreateAsync(TaskItemCreateRequest createRequest);
        Task<TaskItemResponseDto?> UpdateAsync(int id, TaskItemUpdateRequest updateRequest);
        Task<TaskItemResponseDto?> UpdateStatusAsync(int id, TaskItemUpdateStatusRequest updateStatusRequest);
        Task<bool> DeleteAsync(int id);
    }
}
