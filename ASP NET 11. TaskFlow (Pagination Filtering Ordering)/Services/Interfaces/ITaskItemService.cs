using ASP_NET_11._TaskFlow__Pagination_Filtering_Ordering_.Common;
using ASP_NET_11._TaskFlow__Pagination_Filtering_Ordering_.DTOs.TaskItem_DTOs;
using ASP_NET_11._TaskFlow__Pagination_Filtering_Ordering_.Models;

namespace ASP_NET_11._TaskFlow__Pagination_Filtering_Ordering_.Services.Interfaces
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
