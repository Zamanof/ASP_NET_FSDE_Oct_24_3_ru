namespace ASP_NET_09._TaskFlow_DTOs.DTOs.TaskItemResponseDto_DTOs;

public class TaskItemCreateRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ProjectId { get; set; }
}
