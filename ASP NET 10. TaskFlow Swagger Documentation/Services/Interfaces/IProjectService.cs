using ASP_NET_10._TaskFlow_Swagger_Documentation.DTOs.Project_DTOs;
using ASP_NET_10._TaskFlow_Swagger_Documentation.Models;

namespace ASP_NET_10._TaskFlow_Swagger_Documentation.Services.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectResponseDto>> GetAllAsync();
    Task<ProjectResponseDto?> GetByIdAsync(int id);
    Task<ProjectResponseDto> CreateAsync(ProjectCreateRequest createRequest);
    Task<ProjectResponseDto?> UpdateAsync(int id, ProjectUpdateRequest updateRequest);
    Task<bool> DeleteAsync(int id);

}
