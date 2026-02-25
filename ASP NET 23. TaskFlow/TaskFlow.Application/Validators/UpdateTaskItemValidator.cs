using FluentValidation;
using TaskFlow.Application.DTOs;
using TaskFlow.Domain;
using TaskStatus = TaskFlow.Domain.TaskStatus;

namespace TaskFlow.Application.Validators;

public class UpdateTaskItemValidator : AbstractValidator<UpdateTaskItemDto>
{
    public UpdateTaskItemValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MinimumLength(3).WithMessage("Task Item title must be at least 3 characters long");

        RuleFor(x => x.Status)
            .Must(s => new[] { TaskStatus.ToDo, TaskStatus.InProgress, TaskStatus.Done }.Contains(s))
            .WithMessage("Status must be one of: 0(ToDo), 1(InProgress), 2(Done)");

        RuleFor(x => x.Priority)
            .Must(p => new[] { TaskPriority.Low, TaskPriority.High, TaskPriority.Medium }.Contains(p))
            .WithMessage("Priority must be one of: 0(Low), 1(Medium), 2(High)");
    }
}
