namespace ASP_NET_21._TaskFlow.BLL.DTOs;

public class ProjectResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int TaskCount { get; set; }
    public string OwnerId { get; set; } = string.Empty;
}
public class CreateProjectDto
{
    /// <summary>
    /// Project name
    /// </summary>
    /// <example>My new project</example>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Project description
    /// </summary>
    /// <example>Description for my project</example>
    public string Description { get; set; } = string.Empty;
}

public class UpdateProjectDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class ProjectMemberResponse
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTimeOffset JoinedAt { get; set; }
}

public class AvailableUserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}

public class AddProjectMemberDto
{
    public string? UserId { get; set; }
    public string? Email { get; set; }
}