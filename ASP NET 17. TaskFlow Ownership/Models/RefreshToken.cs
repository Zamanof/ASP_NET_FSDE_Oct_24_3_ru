namespace ASP_NET_17._TaskFlow_Ownership.Models;

public class RefreshToken
{
    public int Id { get; set; }
    public string JwtId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public string? ReplacedByJwtId { get; set; }

    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsExpired => ExpiresAt <= DateTime.UtcNow;
    public bool IsActive => !IsRevoked && !IsExpired;

}
