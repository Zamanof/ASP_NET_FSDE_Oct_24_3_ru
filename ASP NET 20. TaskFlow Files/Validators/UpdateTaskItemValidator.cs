using ASP_NET_20._TaskFlow_Files.DTOs;
using ASP_NET_20._TaskFlow_Files.Models;
using FluentValidation;
using TaskStatus = ASP_NET_20._TaskFlow_Files.Models.TaskStatus;

namespace ASP_NET_20._TaskFlow_Files.Validators;

public class UpdateTaskItemValidator : AbstractValidator<TaskItemUpdateRequest>
{
    public UpdateTaskItemValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Project Name is required")
            .MinimumLength(3).WithMessage("Project Name must be at least 3 characters long");


        RuleFor(x => x.Priority)
            .Must(p => new[] { TaskPriority.Low, TaskPriority.Medium, TaskPriority.High }.Contains(p))
            .WithMessage("Priority must be one of: 0(Low), 1(Medium), 2(High)");

        RuleFor(x => x.Status)            
            .Must(s => new[] { TaskStatus.ToDo, TaskStatus.InProgress, TaskStatus.Done }.Contains(s))
            .WithMessage("Priority must be one of: 0(Low), 1(Medium), 2(High)");
    }
}
