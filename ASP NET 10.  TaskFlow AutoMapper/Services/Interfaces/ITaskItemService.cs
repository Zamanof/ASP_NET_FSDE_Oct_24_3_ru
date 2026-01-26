using ASP_NET_10.__TaskFlow_AutoMapper.DTOs.TaskItem_DTOs;
using ASP_NET_10.__TaskFlow_AutoMapper.Models;

namespace ASP_NET_10.__TaskFlow_AutoMapper.Services.Interfaces
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
