namespace ASP_NET_21._TaskFlow.Models;

public class ProjectMember
{
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
}
