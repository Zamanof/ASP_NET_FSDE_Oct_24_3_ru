using FluentValidation;
using ASP_NET_23._TaskFlow_CQRS.Application.DTOs;

namespace ASP_NET_23._TaskFlow_CQRS.Application.Validators;

public class UpdateProjectValidator : AbstractValidator<UpdateProjectDto>
{
    public UpdateProjectValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Project Name is required")
            .MinimumLength(3).WithMessage("Project name must be at least 3 characters long");
    }
}
