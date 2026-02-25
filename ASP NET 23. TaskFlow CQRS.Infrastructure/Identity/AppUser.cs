using Microsoft.AspNetCore.Identity;

namespace ASP_NET_23._TaskFlow_CQRS.Infrastructure.Identity;

public class AppUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
