using ASP_NET_13._TaskFLow_Authentication_and_Authorization.DTOs.Project_DTOs;
using ASP_NET_13._TaskFLow_Authentication_and_Authorization.Models;

namespace ASP_NET_13._TaskFLow_Authentication_and_Authorization.Services.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectResponseDto>> GetAllAsync();
    Task<ProjectResponseDto?> GetByIdAsync(int id);
    Task<ProjectResponseDto> CreateAsync(ProjectCreateRequest createRequest);
    Task<ProjectResponseDto?> UpdateAsync(int id, ProjectUpdateRequest updateRequest);
    Task<bool> DeleteAsync(int id);

}
