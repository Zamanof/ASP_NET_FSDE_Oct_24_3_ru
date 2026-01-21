using ASP_NET_07._TaskFlow_Introduction.Models;
using ASP_NET_07._TaskFlow_Introduction.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace ASP_NET_07._TaskFlow_Introduction.Controllers;

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
    public async Task<ActionResult<IEnumerable<Project>>> GetAll()
    {
        var projects = await _projectService.GetAllAsync();
        return Ok(projects);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Project>> GetById(int id)
    {
        var project = await _projectService.GetByIdAsync(id);
        if (project is null)
            return NotFound($"Project with ID {id} not found");
        return Ok(project);
    }

    [HttpPost]
    public async Task<ActionResult<Project>> Create([FromBody]Project project)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var createdProject = await _projectService.CreateAsync(project);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdProject.Id }, 
            createdProject);
    }
}
