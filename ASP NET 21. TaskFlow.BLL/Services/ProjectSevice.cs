using ASP_NET_21._TaskFlow.BLL.DTOs;
using ASP_NET_21._TaskFlow.DAL;
using ASP_NET_21._TaskFlow.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace ASP_NET_21._TaskFlow.BLL.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IProjectMemberRepository _projectMemberRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProjectService(
        IProjectRepository projectRepository,
        IProjectMemberRepository projectMemberRepository,
        IUserRepository userRepository,
        IMapper mapper,
        UserManager<ApplicationUser> userManager)
    {
        _projectRepository = projectRepository;
        _projectMemberRepository = projectMemberRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<IEnumerable<ProjectResponseDto>> GetAllForUserAsync(
        string userId, IList<string> roles)
    {
        var projects = await _projectRepository.GetAllForUserAsync(userId, roles);

        return _mapper.Map<IEnumerable<ProjectResponseDto>>(projects);
    }

    public async Task<Project?> GetProjectEntityAsync(int id)
    {
        return await _projectRepository.GetByIdWithTasksAndMembersAsync(id);
    }

    public async Task<ProjectResponseDto?> GetByIdAsync(int id)
    {
        var project = await _projectRepository.GetByIdWithTasksAsync(id);

        if (project is null) return null;

        return _mapper.Map<ProjectResponseDto>(project);
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
        var updatedProject = await _projectRepository.GetByIdWithTasksAsync(id);

        if (updatedProject is null) return null;

        _mapper.Map(updateProjectDto, updatedProject);

        await _projectRepository.UpdateAsync(updatedProject);

        return _mapper.Map<ProjectResponseDto>(updatedProject);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var project = await _projectRepository.FindAsync(id);
        if (project is null)
            return false;
        await _projectRepository.RemoveAsync(project);
        return true;
    }

    public async Task<IEnumerable<ProjectMemberResponse>> GetMembersAsync(int projectId)
    {
        var members = await _projectMemberRepository.GetByProjectIdWithUserAsync(projectId);

        return members.Select(m => new ProjectMemberResponse
        {
            UserId = m.UserId,
            Email = m.User.Email!,
            FirstName = m.User.FirstName,
            LastName = m.User.LastName,
            JoinedAt = m.CreatedAt
        });
    }

    public async Task<IEnumerable<AvailableUserDto>> GetAvailableUsersToAddAsync(int projectId)
    {
        var memberUserIds = await _projectMemberRepository.GetMemberUserIdsAsync(projectId);
        var users = await _userRepository.GetOrderedByEmailExceptIdsAsync(memberUserIds);

        return users.Select(u => new AvailableUserDto
        {
            Id = u.Id,
            Email = u.Email!,
            FirstName = u.FirstName,
            LastName = u.LastName
        });
    }

    public async Task<bool> AddMemberAsync(int projectId, string userIdOrEmail)
    {
        var project = await _projectRepository.FindAsync(projectId);
        if (project is null) return false;

        ApplicationUser? user = userIdOrEmail.Contains('@')
            ? await _userManager.FindByEmailAsync(userIdOrEmail)
            : await _userManager.FindByIdAsync(userIdOrEmail);

        if (user is null) return false;
        if (await _projectMemberRepository.ExistsAsync(projectId, user.Id))
            return false;

        await _projectMemberRepository.AddAsync(new ProjectMember
        {
            ProjectId = projectId,
            UserId = user.Id,
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

    public async Task<bool> IsMemberAsync(int projectId, string userId)
    {
        return await _projectMemberRepository.ExistsAsync(projectId, userId);
    }

}
