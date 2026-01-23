using ASP_NET_09._TaskFlow_DTOs.DTOs.Project_DTOs;
using ASP_NET_09._TaskFlow_DTOs.Models;

namespace ASP_NET_09._TaskFlow_DTOs.Services.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectResponseDto>> GetAllAsync();
    Task<ProjectResponseDto?> GetByIdAsync(int id);
    Task<ProjectResponseDto> CreateAsync(ProjectCreateRequest createRequest);
    Task<ProjectResponseDto?> UpdateAsync(int id, ProjectUpdateRequest updateRequest);
    Task<bool> DeleteAsync(int id);

}
