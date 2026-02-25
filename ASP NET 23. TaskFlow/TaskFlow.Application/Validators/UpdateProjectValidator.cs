using FluentValidation;
using TaskFlow.Application.DTOs;

namespace TaskFlow.Application.Validators;

public class UpdateProjectValidator : AbstractValidator<UpdateProjectDto>
{
    public UpdateProjectValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Project Name is required")
            .MinimumLength(3).WithMessage("Project name must be at least 3 characters long");
    }
}
