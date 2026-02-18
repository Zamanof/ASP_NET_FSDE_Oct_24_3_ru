namespace ASP_NET_20._TaskFlow_Refactored.Models;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string OwnerId { get; set; } = string.Empty;
    public ApplicationUser Owner { get; set; } = null!;

    public IEnumerable<ProjectMember> Members { get; set; }
        = new List<ProjectMember>();

    public IEnumerable<TaskItem> Tasks { get; set; } 
        = new List<TaskItem>();
}
