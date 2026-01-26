using ASP_NET_09._TaskFlow_DTO.Data;
using ASP_NET_09._TaskFlow_DTO.DTOs.Project_DTOs;
using ASP_NET_09._TaskFlow_DTO.Models;
using ASP_NET_09._TaskFlow_DTO.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ASP_NET_09._TaskFlow_DTO.Services;

public class ProjectService : IProjectService
{
    private readonly TaskFlowDbContext _context;

    public ProjectService(TaskFlowDbContext context)
    {
        _context = context;
    }

    public async Task<ProjectResponseDto> CreateAsync(ProjectCreateRequest createRequest)
    {
        Project project = new()
        {
            Name = createRequest.Name,
            Description = createRequest.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        await _context
                   .Entry(project)
                   .Collection(p => p.Tasks)
                   .LoadAsync();


        return MapToResponseDto(project);

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

        return projects.Select(MapToResponseDto);
    }

    public async Task<ProjectResponseDto?> GetByIdAsync(int id)
    {
        var project = await _context
                        .Projects
                        .Include(p => p.Tasks)
                        .FirstOrDefaultAsync(p => p.Id == id);
        return MapToResponseDto(project!);
    }

    public async Task<ProjectResponseDto?> UpdateAsync(int id, ProjectUpdateRequest updateRequest)
    {
        var updatedProject = await _context
                                        .Projects
                                        .Include(p => p.Tasks)
                                        .FirstOrDefaultAsync(p => p.Id == id);

        if (updatedProject is null) return null;

        updatedProject.Name = updateRequest.Name;
        updatedProject.Description = updateRequest.Description;
        updatedProject.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToResponseDto(updatedProject);
    }

    private ProjectResponseDto MapToResponseDto(Project project)
    {
        return new ProjectResponseDto
        {
            Id = project!.Id,
            Name = project.Name,
            Description = project.Description,
            TaskCount = project.Tasks.Count()
        };
    }
}