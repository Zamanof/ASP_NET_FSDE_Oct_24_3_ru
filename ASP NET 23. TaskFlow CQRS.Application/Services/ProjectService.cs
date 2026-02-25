using AutoMapper;
using ASP_NET_23._TaskFlow_CQRS.Application.DTOs;
using ASP_NET_23._TaskFlow_CQRS.Application.Interfaces;
using ASP_NET_23._TaskFlow_CQRS.Domain;

namespace ASP_NET_23._TaskFlow_CQRS.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IProjectMemberRepository _projectMemberRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAuthUserStore _authUserStore;
    private readonly IMapper _mapper;

    public ProjectService(
        IProjectRepository projectRepository,
        IProjectMemberRepository projectMemberRepository,
        IUserRepository userRepository,
        IAuthUserStore authUserStore,
        IMapper mapper)
    {
        _projectRepository = projectRepository;
        _projectMemberRepository = projectMemberRepository;
        _userRepository = userRepository;
        _authUserStore = authUserStore;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProjectResponseDto>> GetAllForUserAsync(string userId, IList<string> roles)
    {
        var projects = await _projectRepository.GetAllForUserAsync(userId, roles);
        return _mapper.Map<IEnumerable<ProjectResponseDto>>(projects);
    }

    public async Task<Project?> GetProjectEntityAsync(int id) =>
        await _projectRepository.GetByIdWithTasksAndMembersAsync(id);

    public async Task<ProjectResponseDto?> GetByIdAsync(int id)
    {
        var project = await _projectRepository.GetByIdWithTasksAsync(id);
        return project is null ? null : _mapper.Map<ProjectResponseDto>(project);
    }

    public async Task<ProjectResponseDto> CreateAsync(CreateProjectDto createProjectDto, string ownerId)
    {
        var project = _mapper.Map<Project>(createProjectDto);
        project.OwnerId = ownerId;
        await _projectRepository.AddAsync(project);
        return _mapper.Map<ProjectResponseDto>(project);
    }

    public async Task<ProjectResponseDto?> UpdateAsync(int id, UpdateProjectDto updateProjectDto)
    {
        var project = await _projectRepository.GetByIdWithTasksAsync(id);
        if (project is null) return null;
        _mapper.Map(updateProjectDto, project);
        await _projectRepository.UpdateAsync(project);
        return _mapper.Map<ProjectResponseDto>(project);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var project = await _projectRepository.FindAsync(id);
        if (project is null) return false;
        await _projectRepository.RemoveAsync(project);
        return true;
    }

    public async Task<IEnumerable<ProjectMemberResponse>> GetMembersAsync(int projectId) =>
        await _projectMemberRepository.GetByProjectIdWithUserAsync(projectId);

    public async Task<IEnumerable<AvailableUserDto>> GetAvailableUsersToAddAsync(int projectId)
    {
        var memberUserIds = await _projectMemberRepository.GetMemberUserIdsAsync(projectId);
        return await _userRepository.GetOrderedByEmailExceptIdsAsync(memberUserIds);
    }

    public async Task<bool> AddMemberAsync(int projectId, string userIdOrEmail)
    {
        var project = await _projectRepository.FindAsync(projectId);
        if (project is null) return false;

        var userId = await _authUserStore.FindUserIdByEmailOrIdAsync(userIdOrEmail);
        if (userId is null) return false;
        if (await _projectMemberRepository.ExistsAsync(projectId, userId)) return false;

        await _projectMemberRepository.AddAsync(new ProjectMember
        {
            ProjectId = projectId,
            UserId = userId,
            CreatedAt = DateTimeOffset.UtcNow
        });
        return true;
    }

    public async Task<bool> RemoveMemberAsync(int projectId, string userId)
    {
        var member = await _projectMemberRepository.FindAsync(projectId, userId);
        if (member is null) return false;
        await _projectMemberRepository.RemoveAsync(member);
        return true;
    }

    public Task<bool> IsMemberAsync(int projectId, string userId) =>
        _projectMemberRepository.ExistsAsync(projectId, userId);
}
