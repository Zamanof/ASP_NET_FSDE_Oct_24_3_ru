using ASP_NET_20._TaskFlow_Files.DTOs;
using FluentValidation;

namespace ASP_NET_20._TaskFlow_Files.Validators;

public class CreateProjectValidator : AbstractValidator<ProjectCreateRequest>
{
    public CreateProjectValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Project Name is required")
            .MinimumLength(3).WithMessage("Project Name must be at least 3 characters long");
    }
}
