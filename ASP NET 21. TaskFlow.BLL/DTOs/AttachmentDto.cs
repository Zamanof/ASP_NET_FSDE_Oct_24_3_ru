namespace ASP_NET_21._TaskFlow.BLL.DTOs;

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
