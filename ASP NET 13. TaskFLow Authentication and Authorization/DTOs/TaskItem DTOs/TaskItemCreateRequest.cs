using ASP_NET_13._TaskFLow_Authentication_and_Authorization.Models;

namespace ASP_NET_13._TaskFLow_Authentication_and_Authorization.DTOs.TaskItem_DTOs;

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
