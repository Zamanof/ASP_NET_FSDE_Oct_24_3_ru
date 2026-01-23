using ASP_NET_09._TaskFlow_DTOs.DTOs.TaskItemResponseDto_DTOs;

namespace ASP_NET_09._TaskFlow_DTOs.Services.Interfaces
{
    public interface ITaskItemService
    {
        Task<IEnumerable<TaskItemResponseDto>> GetAllAsync();
        Task<IEnumerable<TaskItemResponseDto>> GetByProjectIdAsync(int projectId);
        Task<TaskItemResponseDto?> GetByIdAsync(int id);
        Task<TaskItemResponseDto> CreateAsync(TaskItemCreateRequest createRequest);
        Task<TaskItemResponseDto?> UpdateAsync(int id, TaskItemUpdateRequest updateRequest );
        Task<bool> DeleteAsync(int id);
    }
}
