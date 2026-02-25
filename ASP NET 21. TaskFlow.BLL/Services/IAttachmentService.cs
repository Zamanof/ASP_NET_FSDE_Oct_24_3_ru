using ASP_NET_21._TaskFlow.BLL.DTOs;

namespace ASP_NET_21._TaskFlow.BLL.Services;

public interface ITaskAttachmentService
{
    Task<AttachmentResponseDto?> UploadAsync(
        int taskId,
        Stream stream,
        string originalFileName,
        string contentType,
        long length,
        string userId,
        CancellationToken cancellationToken = default
        );

    Task<(Stream stream, string fileName, string contentType)?>GetDownloadAsync(
       int attachmentId,
       CancellationToken cancellationToken=default
       );

    Task<bool> DeleteAsync(
       int attachmentId,
       CancellationToken cancellationToken = default
       );

    Task<TaskAttachmentInfo?> GetAttachmentInfoAsync(
       int attachmentId,
       CancellationToken cancellationToken = default
       );
}

public class TaskAttachmentInfo
{
    public int Id { get; set; }
    public int TaskItemId { get; set; }
    public int ProjectId { get; set; }

    public string StoredFileName { get; set; } = string.Empty;
    public string StorageKey { get; set; } = string.Empty;
    public string UploadedByUserId { get; set; } = string.Empty;
}