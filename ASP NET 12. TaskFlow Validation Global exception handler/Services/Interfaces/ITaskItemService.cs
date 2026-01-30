using ASP_NET_12._TaskFlow_Validation_Global_exception_handler.Common;
using ASP_NET_12._TaskFlow_Validation_Global_exception_handler.DTOs.TaskItem_DTOs;
using ASP_NET_12._TaskFlow_Validation_Global_exception_handler.Models;

namespace ASP_NET_12._TaskFlow_Validation_Global_exception_handler.Services.Interfaces
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
