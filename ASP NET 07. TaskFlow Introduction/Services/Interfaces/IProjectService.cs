using ASP_NET_07._TaskFlow_Introduction.Models;

namespace ASP_NET_07._TaskFlow_Introduction.Services.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<Project>> GetAllAsync();
    Task<Project?> GetByIdAsync(int id);
    Task<Project> CreateAsync(Project project);
    Task<Project?> UpdateAsync(int id, Project project);
    Task<bool> DeleteAsync(int id);

}
