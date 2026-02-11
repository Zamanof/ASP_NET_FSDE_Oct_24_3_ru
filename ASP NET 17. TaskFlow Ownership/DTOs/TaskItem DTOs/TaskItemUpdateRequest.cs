using ASP_NET_17._TaskFlow_Ownership.Models;

namespace ASP_NET_17._TaskFlow_Ownership.DTOs.TaskItem_DTOs;

public class TaskItemUpdateRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Models.TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
}
