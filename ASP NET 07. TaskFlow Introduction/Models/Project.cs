namespace ASP_NET_07._TaskFlow_Introduction.Models;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public IEnumerable<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
