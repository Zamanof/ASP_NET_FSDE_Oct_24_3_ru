using ASP_NET_17._TaskFlow_Ownership.DTOs.Project_DTOs;
using ASP_NET_17._TaskFlow_Ownership.Models;

namespace ASP_NET_17._TaskFlow_Ownership.Services.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectResponseDto>> GetAllAsync();
    Task<ProjectResponseDto?> GetByIdAsync(int id);
    Task<ProjectResponseDto> CreateAsync(ProjectCreateRequest createRequest);
    Task<ProjectResponseDto?> UpdateAsync(int id, ProjectUpdateRequest updateRequest);
    Task<bool> DeleteAsync(int id);

}
