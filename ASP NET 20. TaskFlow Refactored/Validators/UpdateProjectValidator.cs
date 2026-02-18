using ASP_NET_20._TaskFlow_Refactored.DTOs;
using FluentValidation;

namespace ASP_NET_20._TaskFlow_Refactored.Validators;

public class UpdateProjectValidator : AbstractValidator<ProjectUpdateRequest>
{
    public UpdateProjectValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Project Name is required")
            .MinimumLength(3).WithMessage("Project Name must be at least 3 characters long");
    }
}
