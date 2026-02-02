using ASP_NET_13._TaskFLow_Authentication_and_Authorization.Models;

namespace ASP_NET_13._TaskFLow_Authentication_and_Authorization.DTOs.TaskItem_DTOs;

public class TaskItemUpdateRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Models.TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
}
