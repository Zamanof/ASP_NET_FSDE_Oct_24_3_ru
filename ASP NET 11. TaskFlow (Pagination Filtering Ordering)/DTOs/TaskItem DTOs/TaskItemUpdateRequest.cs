using ASP_NET_11._TaskFlow__Pagination_Filtering_Ordering_.Models;

namespace ASP_NET_11._TaskFlow__Pagination_Filtering_Ordering_.DTOs.TaskItem_DTOs;

public class TaskItemUpdateRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Models.TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
}
