namespace ASP_NET_23._TaskFlow_CQRS.Domain;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskStatus Status { get; set; } = TaskStatus.ToDo;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public ICollection<TaskAttachment> Attachments { get; set; } = new List<TaskAttachment>();
}
