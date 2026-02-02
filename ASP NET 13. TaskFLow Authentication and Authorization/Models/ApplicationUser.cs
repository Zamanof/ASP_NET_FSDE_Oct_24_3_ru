using Microsoft.AspNetCore.Identity;

namespace ASP_NET_13._TaskFLow_Authentication_and_Authorization.Models;

public class ApplicationUser: IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; } = null!;
}

// OAuth 2