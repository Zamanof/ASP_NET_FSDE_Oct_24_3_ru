namespace ASP_NET_10._TaskFlow_Swagger_Documentation.DTOs.TaskItem_DTOs;

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
    public int ProjectId { get; set; }
}
