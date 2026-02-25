using ASP_NET_23._TaskFlow_CQRS.Domain;

namespace ASP_NET_23._TaskFlow_CQRS.Application.Interfaces;

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
