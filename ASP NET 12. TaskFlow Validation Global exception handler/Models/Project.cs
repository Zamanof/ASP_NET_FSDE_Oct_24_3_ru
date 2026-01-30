using System.Text.Json.Serialization;

namespace ASP_NET_12._TaskFlow_Validation_Global_exception_handler.Models;

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
