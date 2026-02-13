using ASP_NET_17._TaskFlow_Ownership.Data;
using ASP_NET_17._TaskFlow_Ownership.DTOs.Project_DTOs;
using ASP_NET_17._TaskFlow_Ownership.Models;
using ASP_NET_17._TaskFlow_Ownership.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ASP_NET_17._TaskFlow_Ownership.Services;

public class ProjectService : IProjectService
{
    private readonly TaskFlowDbContext _context;
    private readonly IMapper _mapper;

    public ProjectService(TaskFlowDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<bool> AddMemberAsync(int projectId, string userIdOrEmail)
    {
        var project = _context.Projects.FirstOrDefault(p => p.Id == projectId);
        
        if (project is null) return false;

        ApplicationUser? user = null;
        if(userIdOrEmail.Contains("@"))
        {
            user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userIdOrEmail);
        }
        else
        {
            user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userIdOrEmail);
        }
        if (user == null) return false;

        if(await _context.ProjectMembers.AnyAsync(pm => pm.ProjectId == projectId && pm.UserId == user.Id))        
            return false;
        
        _context.ProjectMembers.Add(new ProjectMember
        {
            ProjectId = projectId,
            UserId = user.Id,
            CreatedAt = DateTimeOffset.UtcNow
        });
        
        await _context.SaveChangesAsync();
        return true;


    }

    public async Task<ProjectResponseDto> CreateAsync(ProjectCreateRequest createRequest, string ownerId)
    {
        var project = _mapper.Map<Project>(createRequest);
        project.OwnerId = ownerId;

        _context.Projects.Add(project);
        await _context.SaveChangesAsync() ;

        await _context.Entry(project).Collection(p => p.Tasks).LoadAsync();

        return _mapper.Map<ProjectResponseDto>(project);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var project = await _context.Projects.FindAsync(id);

        if (project is null) return false;

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();

        return true;

    }   

    public async Task<IEnumerable<ProjectResponseDto>> GetAllForUserAsync(string userId, IList<string> roles)
    {
        IQueryable<Project> query = _context.Projects
                                     .Include(p => p.Tasks);
        if (roles.Contains("Admin"))
        {
        }
        else if(roles.Contains("Manager"))
        {
            query = query.Where(p => p.OwnerId == userId || p.Members.Any(m => m.UserId == userId));
        }
        else
        {
            query = query.Where(p => p.Members.Any(m => m.UserId == userId));
        }
        var projects = await query.ToListAsync();
        return _mapper.Map<IEnumerable<ProjectResponseDto>>(projects);


    }

    public async Task<IEnumerable<AvailableUserDto>> GetAvailableUsersToAddAsync(int projectId)
    {
        var membersIds = await _context.ProjectMembers
                                .Where(pm => pm.ProjectId == projectId)
                                .Select(pm => pm.UserId)
                                .ToListAsync();
        var users = await _context.Users
                            .Where(u => !membersIds.Contains(u.Id))
                            .OrderBy(u => u.Email)
                            .Select(u => new AvailableUserDto
                            {
                                Id = u.Id,
                                Email = u.Email!,
                                FirstName = u.FirstName,
                                LastName = u.LastName
                            })
                            .ToListAsync();
        return users;
    }

    public async Task<ProjectResponseDto?> GetByIdAsync(int id)
    {
        var project = await _context
                        .Projects
                        .Include(p => p.Tasks)
                        .FirstOrDefaultAsync(p => p.Id == id);
        return _mapper.Map<ProjectResponseDto>(project);
    }

    public Task<Project?> GetProjectEntityAsync(int id)
    {
        return _context.Projects
                       .Include(p => p.Tasks)
                       .Include(p => p.Members)
                       .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<ProjectMemberResponseDto>> GetProjectMembersAsync(int projectId)
    {
        var members = await _context.ProjectMembers
                              .Include(pm => pm.User)
                              .Where(pm => pm.ProjectId == projectId)
                              .OrderBy(pm => pm.CreatedAt)
                              .ToListAsync();

        return members.Select(m => new ProjectMemberResponseDto
        {
            UserId = m.UserId,
            Email = m.User.Email!,
            FirstName = m.User.FirstName,
            LastName = m.User.LastName,
            JoinedAt = m.CreatedAt
        });
    }

    public async Task<bool> RemoveMemberAsync(int projectId, string userId)
    {
        var member = _context.ProjectMembers
            .FirstOrDefault(pm => pm.ProjectId == projectId && pm.UserId == userId);

        if (member is null) return false;

        _context.ProjectMembers.Remove(member);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<ProjectResponseDto?> UpdateAsync(int id, ProjectUpdateRequest updateRequest)
    {
        var updatedProject = await _context
                                        .Projects
                                        .Include(p => p.Tasks)
                                        .FirstOrDefaultAsync(p => p.Id == id);

        if (updatedProject is null) return null;

       _mapper.Map(updateRequest, updatedProject);

        await _context.SaveChangesAsync();

        return _mapper.Map<ProjectResponseDto>(updatedProject);
    }
}