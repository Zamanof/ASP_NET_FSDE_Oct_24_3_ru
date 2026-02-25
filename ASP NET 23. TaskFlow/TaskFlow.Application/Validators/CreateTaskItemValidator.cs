using FluentValidation;
using TaskFlow.Application.DTOs;
using TaskFlow.Domain;

namespace TaskFlow.Application.Validators;

public class CreateTaskItemValidator : AbstractValidator<CreateTaskItemDto>
{
    public CreateTaskItemValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MinimumLength(3).WithMessage("Task Item title must be at least 3 characters long");

        RuleFor(x => x.ProjectId)
            .GreaterThan(0).WithMessage("ProjectId must be greater than 0");

        RuleFor(x => x.Priority)
            .Must(p => new[] { TaskPriority.Low, TaskPriority.High, TaskPriority.Medium }.Contains(p))
            .WithMessage("Priority must be one of: 0(Low), 1(Medium), 2(High)");
    }
}
