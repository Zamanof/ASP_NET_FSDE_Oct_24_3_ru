using System.Text.Json.Serialization;

namespace ASP_NET_09._TaskFlow_DTO.Models;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    [JsonIgnore]
    public IEnumerable<TaskItem> Tasks { get; set; } 
        = new List<TaskItem>();
}
