namespace ASP_NET_23._TaskFlow_CQRS.Application.DTOs;

public class AttachmentResponseDto
{
    public int Id { get; set; }
    public int TaskItemId { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }
    public string UploadedByUserId { get; set; } = string.Empty;
    public DateTimeOffset UploadedAt { get; set; }
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
