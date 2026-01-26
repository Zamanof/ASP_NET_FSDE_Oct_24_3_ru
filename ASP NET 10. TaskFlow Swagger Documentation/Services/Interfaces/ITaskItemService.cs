using ASP_NET_10._TaskFlow_Swagger_Documentation.DTOs.TaskItem_DTOs;
using ASP_NET_10._TaskFlow_Swagger_Documentation.Models;

namespace ASP_NET_10._TaskFlow_Swagger_Documentation.Services.Interfaces
{
    public interface ITaskItemService
    {
        Task<IEnumerable<TaskItemResponseDto>> GetAllAsync();
        Task<IEnumerable<TaskItemResponseDto>> GetByProjectIdAsync(int projectId);
        Task<TaskItemResponseDto?> GetByIdAsync(int id);
        Task<TaskItemResponseDto> CreateAsync(TaskItemCreateRequest createRequest);
        Task<TaskItemResponseDto?> UpdateAsync(int id, TaskItemUpdateRequest updateRequest);
        Task<bool> DeleteAsync(int id);
    }
}
