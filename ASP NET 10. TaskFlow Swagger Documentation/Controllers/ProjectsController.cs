using ASP_NET_10._TaskFlow_Swagger_Documentation.DTOs.Project_DTOs;
using ASP_NET_10._TaskFlow_Swagger_Documentation.Models;
using ASP_NET_10._TaskFlow_Swagger_Documentation.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace ASP_NET_10._TaskFlow_Swagger_Documentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    /// <summary>
    /// Retrieves all projects.
    /// </summary>
    /// <returns>List of all projects in the system</returns>
    /// <response code="200">Successfully retrieved the list of projects</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProjectResponseDto>>> GetAll()
    {
        var projects = await _projectService.GetAllAsync();
        return Ok(projects);
    }


    /// <summary>
    /// Retrieves a project by its identifier.
    /// </summary>
    /// <param name="id">Project identifier</param>
    /// <returns>Project information</returns>
    /// <response code="200">Project successfully found and returned</response>
    /// <response code="404">Project with the specified identifier not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectResponseDto>> GetById(int id)
    {
        var project = await _projectService.GetByIdAsync(id);
        if (project is null)
            return NotFound($"Project with ID {id} not found");
        return Ok(project);
    }

    /// <summary>
    /// Creates a new project.
    /// </summary>
    /// <param name="createRequest">Data for creating a project</param>
    /// <returns>Created project</returns>
    /// <response code="201">Project successfully created</response>
    /// <response code="400">Invalid request data</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProjectResponseDto>> Create([FromBody] ProjectCreateRequest createRequest)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var createdProject = await _projectService.CreateAsync(createRequest);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdProject.Id },
            createdProject);
    }

    /// <summary>
    /// Updates an existing project.
    /// </summary>
    /// <param name="id">Project identifier to update</param>
    /// <param name="updateRequest">Data for updating the project</param>
    /// <returns>Updated project</returns>
    /// <response code="200">Project successfully updated</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="404">Project with the specified identifier not found</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectResponseDto?>> Update(int id, [FromBody] ProjectUpdateRequest updateRequest)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var updatedProject = await _projectService.UpdateAsync(id, updateRequest);

        if (updatedProject is null)
            return NotFound($"Project with ID {id} not found");

        return Ok(updatedProject);
    }

    /// <summary>
    /// Deletes a project by its identifier.
    /// </summary>
    /// <param name="id">Project identifier to delete</param>
    /// <returns>Result of the delete operation</returns>
    /// <response code="204">Project successfully deleted</response>
    /// <response code="404">Project with the specified identifier not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id)
    {
        var isDeleted = await _projectService.DeleteAsync(id);

        if (!isDeleted)
            return NotFound($"Project with ID {id} not found");

        return NoContent();
    }
}
