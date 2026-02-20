using ASP_NET_20._TaskFlow_Files.Common;
using ASP_NET_20._TaskFlow_Files.DTOs;
using ASP_NET_20._TaskFlow_Files.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASP_NET_20._TaskFlow_Files.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "UserOrAbove")]
public class AttachmentsController : ControllerBase
{
    private readonly IAttachmentService _attachmentService;
    private readonly IProjectService _projectService;
    private readonly ITaskItemService _taskService;
    private readonly IAuthorizationService _authorizationService;

    private string? UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);
    public AttachmentsController(
        IAttachmentService attachmentService,
        IProjectService projectService,
        ITaskItemService taskService,
        IAuthorizationService authorizationService)
    {
        _attachmentService = attachmentService;
        _projectService = projectService;
        _taskService = taskService;
        _authorizationService = authorizationService;
    }
    [HttpPost("~/api/tasks/{taskId}/attachments")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ApiResponse<AttachmentResponseDto>>> Upload(
        int taskId,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        var task = await _taskService.GetTaskEntityAsync(taskId);
        if (task == null)
            return NotFound("Task not found.");

        var project = await _projectService.GetProjectEntityAsync(task.ProjectId);

        if (project == null)
            return NotFound("Project not found.");

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectMemberOrHigher");

        if (!authorizationResult.Succeeded)
            return Forbid();

        if (file is null || file.Length == 0)
            return BadRequest("No file uploaded.");

        AttachmentResponseDto? dto;

        await using (var stream = file.OpenReadStream())
        {
            dto = await _attachmentService.UploadAsync(
                taskId,
                stream,
                file.FileName,
                file.ContentType,
                file.Length,
                UserId!,
                cancellationToken);
        }
        if (dto is null)
            return NotFound();

        return Ok(ApiResponse<AttachmentResponseDto>.SuccessResponse(dto, "File uploaded successfully."));
    }

    [HttpGet("{id}/download")]
    public async Task<IActionResult> Download(
        int id,
        CancellationToken cancellationToken)
    {
        var attachmentInfo = await _attachmentService.GetAttachmentInfoAsync(id, cancellationToken);
        if (attachmentInfo == null)
            return NotFound("Attachment not found.");

        var project = await _projectService.GetProjectEntityAsync(attachmentInfo.ProjectId);
        if (project == null)
            return NotFound("Project not found.");

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectMemberOrHigher");
        if (!authorizationResult.Succeeded)
            return Forbid();

        var fileData = await _attachmentService.GetDownloadAsync(id, cancellationToken);
        if (fileData == null)
            return NotFound("File data not found.");

        return File(fileData.Value.stream, fileData.Value.contentType, fileData.Value.fileName);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id,
        CancellationToken cancellationToken)
    {
        var attachmentInfo = await _attachmentService.GetAttachmentInfoAsync(id, cancellationToken);
        if (attachmentInfo == null)
            return NotFound("Attachment not found.");

        var project = await _projectService.GetProjectEntityAsync(attachmentInfo.ProjectId);
        if (project == null)
            return NotFound("Project not found.");

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");
        if (!authorizationResult.Succeeded)
            return Forbid();

        var deleted = await _attachmentService.DeleteAsync(id, cancellationToken);

        if (!deleted) 
            return NotFound();

        return NoContent();
    }
}
