using ASP_NET_20._TaskFlow_Files.DTOs;
using ASP_NET_20._TaskFlow_Files.Models;
using FluentValidation;

namespace ASP_NET_20._TaskFlow_Files.Validators;

public class CreateTaskItemValidator : AbstractValidator<TaskItemCreateRequest>
{
    public CreateTaskItemValidator()
    {
        RuleFor(x=> x.Title)
            .NotEmpty().WithMessage("Project Name is required")
            .MinimumLength(3).WithMessage("Project Name must be at least 3 characters long");

        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("ProjectId is required")
            .GreaterThan(0).WithMessage("ProjectId must be greater than 0");

        RuleFor(x => x.Priority)
            .Must(p => new[] { TaskPriority.Low, TaskPriority.Medium, TaskPriority.High }.Contains(p))
            .WithMessage("Priority must be one of: 0(Low), 1(Medium), 2(High)");
    }
}
