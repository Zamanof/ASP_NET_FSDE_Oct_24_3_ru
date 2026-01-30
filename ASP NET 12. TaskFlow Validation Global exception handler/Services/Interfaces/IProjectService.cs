using ASP_NET_12._TaskFlow_Validation_Global_exception_handler.DTOs.Project_DTOs;
using ASP_NET_12._TaskFlow_Validation_Global_exception_handler.Models;

namespace ASP_NET_12._TaskFlow_Validation_Global_exception_handler.Services.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectResponseDto>> GetAllAsync();
    Task<ProjectResponseDto?> GetByIdAsync(int id);
    Task<ProjectResponseDto> CreateAsync(ProjectCreateRequest createRequest);
    Task<ProjectResponseDto?> UpdateAsync(int id, ProjectUpdateRequest updateRequest);
    Task<bool> DeleteAsync(int id);

}
