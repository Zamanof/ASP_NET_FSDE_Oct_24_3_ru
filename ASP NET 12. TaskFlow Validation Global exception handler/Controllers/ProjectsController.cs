using ASP_NET_12._TaskFlow_Validation_Global_exception_handler.Common;
using ASP_NET_12._TaskFlow_Validation_Global_exception_handler.DTOs.Project_DTOs;
using ASP_NET_12._TaskFlow_Validation_Global_exception_handler.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ASP_NET_12._TaskFlow_Validation_Global_exception_handler.Controllers;

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
    [HttpGet]
    //[Tags("GetAll")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProjectResponseDto>>>> GetAll()
    {
        //throw new NullReferenceException();
        var projects = await _projectService.GetAllAsync();

        return Ok(
            ApiResponse<IEnumerable<ProjectResponseDto>>
                .SuccessResponse(projects, "Projects retrieved successfully")
        );
    }

    /// <summary>
    /// Retrieves a project by its identifier.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ProjectResponseDto>>> GetById(int id)
    {
        var project = await _projectService.GetByIdAsync(id);

        if (project is null)
            return NotFound(
                ApiResponse<ProjectResponseDto>
                    .ErrorResponse($"Project with ID {id} not found")
            );

        return Ok(
            ApiResponse<ProjectResponseDto>
                .SuccessResponse(project, "Project found")
        );
    }

    /// <summary>
    /// Creates a new project.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<ProjectResponseDto>>> Create(
        [FromBody] ProjectCreateRequest createRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(
                ApiResponse<ProjectResponseDto>
                    .ErrorResponse("Invalid request data")
            );

        var createdProject = await _projectService.CreateAsync(createRequest);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdProject.Id },
            ApiResponse<ProjectResponseDto>
                .SuccessResponse(createdProject, "Project created successfully")
        );
    }

    /// <summary>
    /// Updates an existing project.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ProjectResponseDto>>> Update(
        int id,
        [FromBody] ProjectUpdateRequest updateRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(
                ApiResponse<ProjectResponseDto>
                    .ErrorResponse("Invalid request data")
            );

        var updatedProject = await _projectService.UpdateAsync(id, updateRequest);

        if (updatedProject is null)
            return NotFound(
                ApiResponse<ProjectResponseDto>
                    .ErrorResponse($"Project with ID {id} not found")
            );

        return Ok(
            ApiResponse<ProjectResponseDto>
                .SuccessResponse(updatedProject, "Project updated successfully")
        );
    }

    /// <summary>
    /// Deletes a project by its identifier.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        var isDeleted = await _projectService.DeleteAsync(id);

        if (!isDeleted)
            return NotFound(
                ApiResponse<object>
                    .ErrorResponse($"Project with ID {id} not found")
            );

        return Ok(
            ApiResponse<object>
                .SuccessResponse(null, "Project deleted successfully")
        );
    }
}
