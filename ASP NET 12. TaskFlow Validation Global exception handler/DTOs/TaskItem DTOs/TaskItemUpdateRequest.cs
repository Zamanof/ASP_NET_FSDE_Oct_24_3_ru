using ASP_NET_12._TaskFlow_Validation_Global_exception_handler.Models;

namespace ASP_NET_12._TaskFlow_Validation_Global_exception_handler.DTOs.TaskItem_DTOs;

public class TaskItemUpdateRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Models.TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
}
