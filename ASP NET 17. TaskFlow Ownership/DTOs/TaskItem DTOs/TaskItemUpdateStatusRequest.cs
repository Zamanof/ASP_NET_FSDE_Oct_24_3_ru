using ASP_NET_17._TaskFlow_Ownership.Models;
using TaskStatus = ASP_NET_17._TaskFlow_Ownership.Models.TaskStatus;

namespace ASP_NET_17._TaskFlow_Ownership.DTOs.TaskItem_DTOs;

public class TaskItemUpdateStatusRequest
{
    public TaskStatus Status { get; set; } = TaskStatus.ToDo;
}
