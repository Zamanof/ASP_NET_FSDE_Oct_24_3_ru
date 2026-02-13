using ASP_NET_17._TaskFlow_Ownership.DTOs.Project_DTOs;
using ASP_NET_17._TaskFlow_Ownership.Models;

namespace ASP_NET_17._TaskFlow_Ownership.Services.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectResponseDto>> GetAllForUserAsync(string userId, IList<string> roles);
    Task<ProjectResponseDto?> GetByIdAsync(int id);
    Task<Project?> GetProjectEntityAsync(int id);
    Task<ProjectResponseDto> CreateAsync(ProjectCreateRequest createRequest, string ownerId);
    Task<ProjectResponseDto?> UpdateAsync(int id, ProjectUpdateRequest updateRequest);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<ProjectMemberResponseDto>> GetProjectMembersAsync(int projectId);
    Task<IEnumerable<AvailableUserDto>> GetAvailableUsersToAddAsync(int projectId);
    Task<bool> AddMemberAsync(int projectId, string userIdOrEmail);
    Task<bool> RemoveMemberAsync(int projectId, string userId);

}
