using TaskFlow.Domain;

namespace TaskFlow.Application.Interfaces;

public interface IProjectRepository
{
    Task<Project?> GetByIdWithTasksAsync(int id);
    Task<Project?> GetByIdWithTasksAndMembersAsync(int id);
    Task<Project?> GetByTaskIdAsync(int taskId);
    Task<IEnumerable<Project>?> GetAllForUserAsync(string userId, IList<string> roles);
    Task<Project> AddAsync(Project project);
    Task UpdateAsync(Project project);
    Task RemoveAsync(Project project);
    Task<Project?> FindAsync(int id);
}
