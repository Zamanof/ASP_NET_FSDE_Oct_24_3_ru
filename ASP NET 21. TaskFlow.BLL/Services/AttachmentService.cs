using ASP_NET_21._TaskFlow.BLL.DTOs;
using ASP_NET_21._TaskFlow.BLL.Storage;
using ASP_NET_21._TaskFlow.DAL;
using ASP_NET_21._TaskFlow.Models;

namespace ASP_NET_21._TaskFlow.BLL.Services;

public class TaskAttachmentService : ITaskAttachmentService
{
    public const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5MB
    public static readonly string[] AllowedExtensions = {
        ".jpg", ".jpeg", ".png", ".pdf", ".txt", ".zip",
    };

    public static readonly string[] AllowedContentTypes = {
        "image/jpeg", "image/png", "application/pdf", "text/plain",
        "application/zip", "application/x-zip-compressed"
    };

    private readonly ITaskItemRepository _taskItemRepository;
    private readonly ITaskAttachmentRepository _taskAttachmentRepository;
    private readonly IFileStorage _storage;

    public TaskAttachmentService(
        ITaskItemRepository taskItemRepository,
        ITaskAttachmentRepository taskAttachmentRepository,
        IFileStorage storage)
    {
        _taskItemRepository = taskItemRepository;
        _taskAttachmentRepository = taskAttachmentRepository;
        _storage = storage;
    }

    public async Task<AttachmentResponseDto?> UploadAsync(int taskId, Stream stream, string originalFileName, string contentType, long length, string userId, CancellationToken cancellationToken = default)
    {
        if (length > MaxFileSizeBytes)
            throw new ArgumentException($"File size must not exceed {MaxFileSizeBytes / (1024 * 1024)} MB");

        var ext = Path.GetExtension(originalFileName)?.ToLowerInvariant();
        if (string.IsNullOrEmpty(ext) || !AllowedExtensions.Contains(ext))
            throw new ArgumentException($"Allowed extensions: {string.Join(", ", AllowedExtensions)}");

        if (!AllowedContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase))
            throw new ArgumentException($"Allowed content type: {string.Join(", ", AllowedContentTypes)}");

        var task = await _taskItemRepository.FindAsync(taskId);
        if (task is null)
            throw new ArgumentException($"Task with ID {taskId} not found.");

        var folderKey = $"tasks/{taskId}";
        var info = await _storage.UploadAsync(stream, originalFileName, contentType, folderKey, cancellationToken);

        var attachment = new TaskAttachment
        {
            TaskItemId = taskId,
            OriginalFileName = originalFileName,
            StoredFileName = info.StoredFileName,
            ContentType = contentType,
            Size = info.Size,
            UploadedByUserId = userId,
            UploadedAt = DateTimeOffset.UtcNow
        };

        await _taskAttachmentRepository.AddAsync(attachment);

        return new AttachmentResponseDto
        {
            Id = attachment.Id,
            TaskItemId = attachment.TaskItemId,
            OriginalFileName = attachment.OriginalFileName,
            ContentType = attachment.ContentType,
            Size = attachment.Size,
            UploadedByUserId = attachment.UploadedByUserId,
            UploadedAt = attachment.UploadedAt
        };
    }

    public async Task<(Stream stream, string fileName, string contentType)?> GetDownloadAsync(int attachmentId, CancellationToken cancellationToken = default)
    {
        var att = await _taskAttachmentRepository.GetByIdAsync(attachmentId);
        if (att is null) return null;

        var key = $"tasks/{att.TaskItemId}/{att.StoredFileName}";
        var stream = await _storage.OpenReadAsync(key, cancellationToken);
        return (stream, att.OriginalFileName, att.ContentType);
    }

    public async Task<TaskAttachmentInfo?> GetAttachmentInfoAsync(int attachmentId, CancellationToken cancellationToken = default)
    {
        var att = await _taskAttachmentRepository.GetByIdWithTaskItemAsync(attachmentId);
        if (att is null) return null;

        return new TaskAttachmentInfo
        {
            Id = att.Id,
            TaskItemId = att.TaskItemId,
            ProjectId = att.TaskItem.ProjectId,
            StoredFileName = att.StoredFileName,
            StorageKey = $"tasks/{att.TaskItemId}/{att.StoredFileName}",
            UploadedByUserId = att.UploadedByUserId,
        };
    }

    public async Task<bool> DeleteAsync(int attachmentId, CancellationToken cancellationToken = default)
    {
        var att = await _taskAttachmentRepository.GetByIdAsync(attachmentId);
        if (att is null) return false;

        var key = $"tasks/{att.TaskItemId}/{att.StoredFileName}";
        await _taskAttachmentRepository.RemoveAsync(att);
        await _storage.DeleteAsync(key, cancellationToken);
        return true;
    }
}
