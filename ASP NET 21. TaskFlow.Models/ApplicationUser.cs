using Microsoft.AspNetCore.Identity;

namespace ASP_NET_21._TaskFlow.Models;

public class ApplicationUser: IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; } = null;
    public ICollection<ProjectMember> ProjectMemberships { get; set; } 
            = new List<ProjectMember>();
}
