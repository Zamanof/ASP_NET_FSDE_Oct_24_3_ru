using ASP_NET_20._TaskFlow_Files.DTOs;

namespace ASP_NET_20._TaskFlow_Files.Services;
public interface IAttachmentService
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

    Task<(Stream stream, string fileName, string contentType)?> GetDownloadAsync(
        int attachmentId,
        CancellationToken cancellationToken = default
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
    public int ProjectId { get; set; }
    public int TaskItemId { get; set; }
    public string StorageKey { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public string UploadedByUserId { get; set; } = string.Empty;
}
