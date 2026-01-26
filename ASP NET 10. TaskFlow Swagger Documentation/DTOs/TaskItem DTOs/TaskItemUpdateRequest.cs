using ASP_NET_10._TaskFlow_Swagger_Documentation.Models;

namespace ASP_NET_10._TaskFlow_Swagger_Documentation.DTOs.TaskItem_DTOs;

public class TaskItemUpdateRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Models.TaskStatus Status { get; set; }
}
