using System.Text.Json.Serialization;

namespace ASP_NET_15._TaskFlow_Refresh_Token.Models;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public IEnumerable<TaskItem> Tasks { get; set; } 
        = new List<TaskItem>();
}
