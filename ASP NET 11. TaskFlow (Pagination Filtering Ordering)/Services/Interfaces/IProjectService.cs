using ASP_NET_11._TaskFlow__Pagination_Filtering_Ordering_.DTOs.Project_DTOs;
using ASP_NET_11._TaskFlow__Pagination_Filtering_Ordering_.Models;

namespace ASP_NET_11._TaskFlow__Pagination_Filtering_Ordering_.Services.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectResponseDto>> GetAllAsync();
    Task<ProjectResponseDto?> GetByIdAsync(int id);
    Task<ProjectResponseDto> CreateAsync(ProjectCreateRequest createRequest);
    Task<ProjectResponseDto?> UpdateAsync(int id, ProjectUpdateRequest updateRequest);
    Task<bool> DeleteAsync(int id);

}
