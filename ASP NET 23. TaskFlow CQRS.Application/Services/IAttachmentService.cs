using ASP_NET_23._TaskFlow_CQRS.Application.DTOs;

namespace ASP_NET_23._TaskFlow_CQRS.Application.Services;

public interface IAttachmentService
{
    Task<AttachmentResponseDto?> UploadAsync(
        int taskId,
        Stream stream,
        string originalFileName,
        string contentType,
        long length,
        string userId,
        CancellationToken cancellationToken = default);

    Task<(Stream stream, string fileName, string contentType)?> GetDownloadAsync(
        int attachmentId,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int attachmentId, CancellationToken cancellationToken = default);
    Task<TaskAttachmentInfo?> GetAttachmentInfoAsync(int attachmentId, CancellationToken cancellationToken = default);
}
