namespace ASP_NET_11._TaskFlow__Pagination_Filtering_Ordering_.DTOs.Project_DTOs;

public class ProjectResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int TaskCount { get; set; }

}
