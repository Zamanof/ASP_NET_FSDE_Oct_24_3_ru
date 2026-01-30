using ASP_NET_12._TaskFlow_Validation_Global_exception_handler.Models;

namespace ASP_NET_12._TaskFlow_Validation_Global_exception_handler.DTOs.TaskItem_DTOs;

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
