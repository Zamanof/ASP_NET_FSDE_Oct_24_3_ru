namespace ASP_NET_23._TaskFlow_CQRS.Application.DTOs;

public class TaskItemQueryParams
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public string? Sort { get; set; }
    public string? SortDirection { get; set; } = "asc";
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public string? Search { get; set; }
    public int? ProjectId { get; set; }

    public void Validate()
    {
        if (Page < 1) Page = 1;
        if (Size < 1) Size = 10;
        if (Size > 100) Size = 100;
        if (string.IsNullOrWhiteSpace(SortDirection)) SortDirection = "asc";
        SortDirection = SortDirection.ToLower();
        if (SortDirection != "asc" && SortDirection != "desc") SortDirection = "asc";
    }
}
