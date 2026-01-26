namespace ASP_NET_10.__TaskFlow_AutoMapper.DTOs.Project_DTOs;

public class ProjectResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int TaskCount { get; set; }

}
