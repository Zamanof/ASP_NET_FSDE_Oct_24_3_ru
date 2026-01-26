using ASP_NET_09._TaskFlow_DTO.DTOs.Project_DTOs;
using ASP_NET_09._TaskFlow_DTO.Models;
using ASP_NET_09._TaskFlow_DTO.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace ASP_NET_09._TaskFlow_DTO.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectResponseDto>>> GetAll()
    {
        var projects = await _projectService.GetAllAsync();
        return Ok(projects);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectResponseDto>> GetById(int id)
    {
        var project = await _projectService.GetByIdAsync(id);
        if (project is null)
            return NotFound($"Project with ID {id} not found");
        return Ok(project);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectResponseDto>> Create([FromBody] ProjectCreateRequest createRequest)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var createdProject = await _projectService.CreateAsync(createRequest);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdProject.Id },
            createdProject);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProjectResponseDto?>> Update(int id, [FromBody] ProjectUpdateRequest updateRequest)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var updatedProject = await _projectService.UpdateAsync(id, updateRequest);

        if (updatedProject is null)
            return NotFound($"Project with ID {id} not found");

        return Ok(updatedProject);
    }

    [HttpDelete("{id}")]
    // base_path/id
    public async Task<ActionResult> Delete(int id)
    {
        var isDeleted = await _projectService.DeleteAsync(id);

        if (!isDeleted)
            return NotFound($"Project with ID {id} not found");

        return NoContent();
    }
}
