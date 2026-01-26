using ASP_NET_09._TaskFlow_DTO.DTOs.Project_DTOs;
using ASP_NET_09._TaskFlow_DTO.Models;

namespace ASP_NET_09._TaskFlow_DTO.Services.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectResponseDto>> GetAllAsync();
    Task<ProjectResponseDto?> GetByIdAsync(int id);
    Task<ProjectResponseDto> CreateAsync(ProjectCreateRequest createRequest);
    Task<ProjectResponseDto?> UpdateAsync(int id, ProjectUpdateRequest updateRequest);
    Task<bool> DeleteAsync(int id);

}
