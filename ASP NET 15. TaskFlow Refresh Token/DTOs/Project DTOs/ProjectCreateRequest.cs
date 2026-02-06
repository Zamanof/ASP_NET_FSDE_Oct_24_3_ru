namespace ASP_NET_15._TaskFlow_Refresh_Token.DTOs.Project_DTOs;

/// <summary>
/// DTO for project create. Uses for POST requests
/// </summary>
public class ProjectCreateRequest
{
    /// <summary>
    /// Project Name
    /// </summary>
    /// <example>My new project</example>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Project Description
    /// </summary>
    /// <example>Description for my project</example>
    public string Description { get; set; } = string.Empty;
}
