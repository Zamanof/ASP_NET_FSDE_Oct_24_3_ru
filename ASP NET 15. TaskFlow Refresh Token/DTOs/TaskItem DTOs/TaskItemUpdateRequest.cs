using ASP_NET_15._TaskFlow_Refresh_Token.Models;

namespace ASP_NET_15._TaskFlow_Refresh_Token.DTOs.TaskItem_DTOs;

public class TaskItemUpdateRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Models.TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
}
