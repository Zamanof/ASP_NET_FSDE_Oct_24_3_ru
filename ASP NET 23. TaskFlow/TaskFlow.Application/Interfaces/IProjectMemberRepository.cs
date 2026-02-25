using TaskFlow.Application.DTOs;
using TaskFlow.Domain;

namespace TaskFlow.Application.Interfaces;

public interface IProjectMemberRepository
{
    Task<IEnumerable<ProjectMemberResponse>> GetByProjectIdWithUserAsync(int projectId);
    Task<IEnumerable<string>> GetMemberUserIdsAsync(int projectId);
    Task<bool> ExistsAsync(int projectId, string userId);
    Task<ProjectMember?> FindAsync(int projectId, string userId);
    Task AddAsync(ProjectMember member);
    Task RemoveAsync(ProjectMember member);
}
