using ASP_NET_20._TaskFlow_Files.Models;
using TaskStatus = ASP_NET_20._TaskFlow_Files.Models.TaskStatus;

namespace ASP_NET_20._TaskFlow_Files.DTOs;

public class TaskItemResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}

public class TaskItemCreateRequest
{
    /// <summary>
    /// Task title
    /// </summary>
    /// <example>Do something</example>
    public string Title { get; set; } = string.Empty;
    /// <summary>
    /// Task Description
    /// </summary>
    /// <example>Task full description</example>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Project Id
    /// </summary>
    /// <example>1</example>

    public TaskPriority Priority { get; set; }
    public int ProjectId { get; set; }
}

public class TaskItemUpdateRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Models.TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
}

public class TaskItemUpdateStatusRequest
{
    public TaskStatus Status { get; set; } = TaskStatus.ToDo;
}
