using ASP_NET_15._TaskFlow_Refresh_Token.DTOs.Project_DTOs;
using ASP_NET_15._TaskFlow_Refresh_Token.Models;

namespace ASP_NET_15._TaskFlow_Refresh_Token.Services.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectResponseDto>> GetAllAsync();
    Task<ProjectResponseDto?> GetByIdAsync(int id);
    Task<ProjectResponseDto> CreateAsync(ProjectCreateRequest createRequest);
    Task<ProjectResponseDto?> UpdateAsync(int id, ProjectUpdateRequest updateRequest);
    Task<bool> DeleteAsync(int id);

}
