namespace ASP_NET_09._TaskFlow_DTO.Models;
public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskStatus Status { get; set; } = TaskStatus.ToDo;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    // foreign key
    public int ProjectId { get; set; }

    public Project Project { get; set; } = null!;
}

public enum TaskStatus
{
    ToDo,
    InProgress,
    Done
}
