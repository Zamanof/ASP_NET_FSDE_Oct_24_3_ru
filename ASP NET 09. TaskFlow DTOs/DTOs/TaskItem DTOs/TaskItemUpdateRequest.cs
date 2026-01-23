using ASP_NET_09._TaskFlow_DTOs.Models;

namespace ASP_NET_09._TaskFlow_DTOs.DTOs.TaskItemResponseDto_DTOs;

public class TaskItemUpdateRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Models.TaskStatus Status { get; set; }
}
