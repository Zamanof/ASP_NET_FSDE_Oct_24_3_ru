using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Services;

namespace TaskFlow.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "UserOrAbove")]
public class AttachmentsController : ControllerBase
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IAttachmentService _attachmentService;
    private readonly ITaskItemService _taskItemService;
    private readonly IProjectService _projectService;

    private string? UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

    public AttachmentsController(
        IAuthorizationService authorizationService,
        IAttachmentService attachmentService,
        ITaskItemService taskItemService,
        IProjectService projectService)
    {
        _authorizationService = authorizationService;
        _attachmentService = attachmentService;
        _taskItemService = taskItemService;
        _projectService = projectService;
    }

    [HttpPost("~/api/tasks/{taskId}/attachments")]
    public async Task<ActionResult<ApiResponse<AttachmentResponseDto>>> Upload(int taskId, IFormFile file, CancellationToken cancellationToken)
    {
        var task = await _taskItemService.GetTaskEntityAsync(taskId);
        if (task == null) return NotFound("Task not found");
        var project = await _projectService.GetProjectEntityAsync(task.ProjectId);
        if (project == null) return NotFound("Project not found");
        var authResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectMemberOrHigher");
        if (!authResult.Succeeded) return Forbid();

        if (file is null || file.Length == 0)
            return BadRequest();

        using var stream = file.OpenReadStream();
        var result = await _attachmentService.UploadAsync(taskId, stream, file.FileName, file.ContentType, file.Length, UserId!, cancellationToken);
        if (result == null) return BadRequest("Failed to upload attachment");
        return Ok(ApiResponse<AttachmentResponseDto>.SuccessResponse(result, "File uploaded"));
    }

    [HttpGet("{id}/download")]
    public async Task<IActionResult> Download(int id, CancellationToken cancellationToken)
    {
        var attachmentInfo = await _attachmentService.GetAttachmentInfoAsync(id, cancellationToken);
        if (attachmentInfo == null) return NotFound("Attachment not found");
        var project = await _projectService.GetProjectEntityAsync(attachmentInfo.ProjectId);
        if (project == null) return NotFound("Project not found");
        var authResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectMemberOrHigher");
        if (!authResult.Succeeded) return Forbid();
        var fileData = await _attachmentService.GetDownloadAsync(id, cancellationToken);
        if (fileData == null) return NotFound("File data not found");
        return File(fileData.Value.stream, fileData.Value.contentType, fileData.Value.fileName);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var attachmentInfo = await _attachmentService.GetAttachmentInfoAsync(id, cancellationToken);
        if (attachmentInfo == null) return NotFound("Attachment not found");
        var project = await _projectService.GetProjectEntityAsync(attachmentInfo.ProjectId);
        if (project == null) return NotFound("Project not found");
        var authResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");
        if (!authResult.Succeeded) return Forbid();
        var deleted = await _attachmentService.DeleteAsync(id, cancellationToken);
        if (!deleted) return BadRequest("Failed to delete attachment");
        return NoContent();
    }
}
