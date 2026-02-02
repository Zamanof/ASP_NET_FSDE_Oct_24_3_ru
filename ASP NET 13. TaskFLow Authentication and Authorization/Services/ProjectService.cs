using ASP_NET_13._TaskFLow_Authentication_and_Authorization.Data;
using ASP_NET_13._TaskFLow_Authentication_and_Authorization.DTOs.Project_DTOs;
using ASP_NET_13._TaskFLow_Authentication_and_Authorization.Models;
using ASP_NET_13._TaskFLow_Authentication_and_Authorization.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ASP_NET_13._TaskFLow_Authentication_and_Authorization.Services;

public class ProjectService : IProjectService
{
    private readonly TaskFlowDbContext _context;
    private readonly IMapper _mapper;

    public ProjectService(TaskFlowDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ProjectResponseDto> CreateAsync(ProjectCreateRequest createRequest)
    {
        Project project = _mapper.Map<Project>(createRequest);

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        await _context
                   .Entry(project)
                   .Collection(p => p.Tasks)
                   .LoadAsync();


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

    public async Task<IEnumerable<ProjectResponseDto>> GetAllAsync()
    {
        var projects = await _context
                        .Projects
                        .Include(p => p.Tasks)
                        .ToListAsync();

        return _mapper.Map<IEnumerable<ProjectResponseDto>>(projects);
    }

    public async Task<ProjectResponseDto?> GetByIdAsync(int id)
    {
        var project = await _context
                        .Projects
                        .Include(p => p.Tasks)
                        .FirstOrDefaultAsync(p => p.Id == id);
        return _mapper.Map<ProjectResponseDto>(project);
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