using ASP_NET_10.__TaskFlow_AutoMapper.DTOs.Project_DTOs;
using ASP_NET_10.__TaskFlow_AutoMapper.Models;

namespace ASP_NET_10.__TaskFlow_AutoMapper.Services.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectResponseDto>> GetAllAsync();
    Task<ProjectResponseDto?> GetByIdAsync(int id);
    Task<ProjectResponseDto> CreateAsync(ProjectCreateRequest createRequest);
    Task<ProjectResponseDto?> UpdateAsync(int id, ProjectUpdateRequest updateRequest);
    Task<bool> DeleteAsync(int id);

}
