namespace ASP_NET_11._TaskFlow__Pagination_Filtering_Ordering_.DTOs.TaskItem_DTOs;

public class TaskItemResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}
