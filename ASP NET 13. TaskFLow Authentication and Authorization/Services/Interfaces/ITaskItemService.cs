using ASP_NET_13._TaskFLow_Authentication_and_Authorization.Common;
using ASP_NET_13._TaskFLow_Authentication_and_Authorization.DTOs.TaskItem_DTOs;
using ASP_NET_13._TaskFLow_Authentication_and_Authorization.Models;

namespace ASP_NET_13._TaskFLow_Authentication_and_Authorization.Services.Interfaces
{
    public interface ITaskItemService
    {
        Task<IEnumerable<TaskItemResponseDto>> GetAllAsync();
        Task<PagedResult<TaskItemResponseDto>> GetPagedAsync(TaskItemQueryParams queryParams);
        Task<IEnumerable<TaskItemResponseDto>> GetByProjectIdAsync(int projectId);
        Task<TaskItemResponseDto?> GetByIdAsync(int id);
        Task<TaskItemResponseDto> CreateAsync(TaskItemCreateRequest createRequest);
        Task<TaskItemResponseDto?> UpdateAsync(int id, TaskItemUpdateRequest updateRequest);
        Task<bool> DeleteAsync(int id);
    }
}
