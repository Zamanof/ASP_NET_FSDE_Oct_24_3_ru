using ASP_NET_20._TaskFlow_Files.Data;
using ASP_NET_20._TaskFlow_Files.DTOs;
using ASP_NET_20._TaskFlow_Files.Models;
using ASP_NET_20._TaskFlow_Files.Storage;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ASP_NET_20._TaskFlow_Files.Services;
public class AttachmentService : IAttachmentService
{
    public const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB
    public static readonly string[] AllowedExtensions =
        {
        ".jpg",
        ".jpeg",
        ".png",
        ".pdf",
        ".txt",
        ".zip",
    };
    public static readonly string[] AllowedContentTypes = {
        "image/jpeg",
        "image/png",
        "application/pdf",
        "text/plain",
        "application/zip",
        "application/x-zip-compressed"
    };

    private readonly TaskFlowDbContext _context;
    private readonly IFileStorage _fileStorage;

    public AttachmentService(TaskFlowDbContext context, IFileStorage fileStorage)
    {
        _context = context;
        _fileStorage = fileStorage;
    }

    public async Task<AttachmentResponseDto?> UploadAsync(
        int taskId,
        Stream stream,
        string originalFileName,
        string contentType,
        long length,
        string userId,
        CancellationToken cancellationToken = default)
    {
        if (length > MaxFileSizeBytes)
            throw new ArgumentException($"File size exceeds the maximum allowed size of {MaxFileSizeBytes / (1024 * 1024)} MB.");

        var extension = Path.GetExtension(originalFileName).ToLowerInvariant();

        if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
            throw new ArgumentException($"File extension '{extension}' is not allowed. Allowed extensions are: {string.Join(", ", AllowedExtensions)}.");

        if (!AllowedContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase))
            throw new ArgumentException($"Content type '{contentType}' is not allowed. Allowed content types are: {string.Join(", ", AllowedContentTypes)}.");

        var task = await _context.TaskItems.FindAsync([taskId], cancellationToken);
        if (task == null)
            return null;

        var folderKey = $"task/{taskId}";

        var info = await _fileStorage.UploadAsync(
            stream,
            originalFileName,
            contentType,
            folderKey,
            cancellationToken);

        var attachment = new TaskAttachment
        {
            TaskItemId = taskId,
            StoredFileName = info.StoredFileName,
            OriginalFileName = originalFileName,
            ContentType = contentType,
            Size = length,
            UploadedByUserId = userId,
            UploadedAt = DateTimeOffset.UtcNow
        };

        _context.Attachments.Add(attachment);
        await _context.SaveChangesAsync(cancellationToken);

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
    public async Task<(Stream stream, string fileName, string contentType)?> GetDownloadAsync(
        int attachmentId, CancellationToken cancellationToken = default)
    {
        var attachment = await _context.Attachments.FirstOrDefaultAsync(a => a.Id == attachmentId);

        if (attachment is null)
            return null;
        var key = $"task/{attachment.TaskItemId}/{attachment.StoredFileName}";
        var stream = await _fileStorage.OpenReadAsync(key, cancellationToken);

        return (stream, attachment.OriginalFileName, attachment.ContentType);
    }
    public async Task<TaskAttachmentInfo?> GetAttachmentInfoAsync(int attachmentId, CancellationToken cancellationToken = default)
    {
        var attachment = await _context.Attachments
                                .Include(a=> a.TaskItem)
                                .FirstOrDefaultAsync(a => a.Id == attachmentId);
        if (attachment is null)
            return null;

        return new TaskAttachmentInfo
        {
            Id = attachment.Id,
            ProjectId = attachment.TaskItem.ProjectId,
            TaskItemId = attachment.TaskItemId,
            StorageKey = $"task/{attachment.TaskItemId}/{attachment.StoredFileName}",
            StoredFileName = attachment.StoredFileName,
            UploadedByUserId = attachment.UploadedByUserId
        };

    }
    public async Task<bool> DeleteAsync(int attachmentId, CancellationToken cancellationToken = default)
    {
        var attachment = await _context.Attachments.FirstOrDefaultAsync(a => a.Id == attachmentId);

        if (attachment is null)
            return false;

        var key = $"task/{attachment.TaskItemId}/{attachment.StoredFileName}";

        _context.Attachments.Remove(attachment);
        await _context.SaveChangesAsync(cancellationToken);

        await _fileStorage.DeleteAsync(key, cancellationToken);
        return true;
    }

}
