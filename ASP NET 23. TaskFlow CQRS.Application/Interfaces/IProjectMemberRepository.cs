using ASP_NET_23._TaskFlow_CQRS.Application.DTOs;
using ASP_NET_23._TaskFlow_CQRS.Domain;

namespace ASP_NET_23._TaskFlow_CQRS.Application.Interfaces;

public interface IProjectMemberRepository
{
    Task<IEnumerable<ProjectMemberResponse>> GetByProjectIdWithUserAsync(int projectId);
    Task<IEnumerable<string>> GetMemberUserIdsAsync(int projectId);
    Task<bool> ExistsAsync(int projectId, string userId);
    Task<ProjectMember?> FindAsync(int projectId, string userId);
    Task AddAsync(ProjectMember member);
    Task RemoveAsync(ProjectMember member);
}
