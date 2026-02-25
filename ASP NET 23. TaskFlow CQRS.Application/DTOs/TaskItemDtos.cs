using ASP_NET_23._TaskFlow_CQRS.Domain;
using TaskStatus = ASP_NET_23._TaskFlow_CQRS.Domain.TaskStatus;

namespace ASP_NET_23._TaskFlow_CQRS.Application.DTOs;

public class TaskItemResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}

public class CreateTaskItemDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskPriority Priority { get; set; }
    public int ProjectId { get; set; }
}

public class UpdateTaskItemDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskPriority Priority { get; set; }
    public TaskStatus Status { get; set; }
}

public class TaskStatusUpdateRequest
{
    public TaskStatus Status { get; set; } = TaskStatus.ToDo;
}
