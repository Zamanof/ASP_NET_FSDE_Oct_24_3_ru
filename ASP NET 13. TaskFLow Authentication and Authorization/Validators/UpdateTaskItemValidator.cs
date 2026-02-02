using ASP_NET_13._TaskFLow_Authentication_and_Authorization.DTOs.TaskItem_DTOs;
using ASP_NET_13._TaskFLow_Authentication_and_Authorization.Models;
using FluentValidation;
using TaskStatus = ASP_NET_13._TaskFLow_Authentication_and_Authorization.Models.TaskStatus;

namespace ASP_NET_13._TaskFLow_Authentication_and_Authorization.Validators;

public class UpdateTaskItemValidator : AbstractValidator<TaskItemUpdateRequest>
{
    public UpdateTaskItemValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Project Name is required")
            .MinimumLength(3).WithMessage("Project Name must be at least 3 characters long");


        RuleFor(x => x.Priority)
            .NotEmpty().WithMessage("Priority is required")
            .Must(p => new[] { TaskPriority.Low, TaskPriority.Medium, TaskPriority.High }.Contains(p))
            .WithMessage("Priority must be one of: 0(Low), 1(Medium), 2(High)");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required")
            .Must(s => new[] { TaskStatus.ToDo, TaskStatus.InProgress, TaskStatus.Done }.Contains(s))
            .WithMessage("Priority must be one of: 0(Low), 1(Medium), 2(High)");
    }
}
