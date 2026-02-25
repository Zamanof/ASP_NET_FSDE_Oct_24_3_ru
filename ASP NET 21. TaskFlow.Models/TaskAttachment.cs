namespace ASP_NET_21._TaskFlow.Models;

public class TaskAttachment
{
    public int Id { get; set; }
    public int TaskItemId { get; set; }
    public TaskItem TaskItem { get; set; } = null!;

    public string OriginalFileName { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }


    public string UploadedByUserId { get; set; } = string.Empty;
    public ApplicationUser UploadedByUser { get; set; } = null!;
    public DateTimeOffset UploadedAt { get; set; }
}
