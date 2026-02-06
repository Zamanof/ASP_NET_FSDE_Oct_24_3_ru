using ASP_NET_15._TaskFlow_Refresh_Token.Common;
using ASP_NET_15._TaskFlow_Refresh_Token.DTOs.TaskItem_DTOs;
using ASP_NET_15._TaskFlow_Refresh_Token.Models;

namespace ASP_NET_15._TaskFlow_Refresh_Token.Services.Interfaces
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
