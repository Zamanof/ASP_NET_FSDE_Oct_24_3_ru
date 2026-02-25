using ASP_NET_21._TaskFlow.Models;

namespace ASP_NET_21._TaskFlow.DAL;

public interface ITaskItemRepository
{
    Task<bool> ProjectExistsAsync(int projectId);
    Task<TaskItem> AddAsync(TaskItem taskItem);
    Task<TaskItem?> FindAsync(int id);
    Task<TaskItem?> GetByIdWithProjectAsync(int id);
    Task<IEnumerable<TaskItem>> GetAllWithProjectAsync();
    Task<IEnumerable<TaskItem>> GetByProjectIdAsync(int projectId);
    Task UpdateAsync(TaskItem taskItem);
    Task RemoveAsync(TaskItem taskItem);
    Task<(IEnumerable<TaskItem> Items, int TotalCount)> GetPagedAsync(
        int? projectId,
        string? status,
        string? priority,
        string? search,
        string? sort,
        string? sortDirection,
        int page,
        int size);
}
