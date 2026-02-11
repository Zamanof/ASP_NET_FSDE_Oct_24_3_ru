using ASP_NET_17._TaskFlow_Ownership.DTOs.Project_DTOs;
using FluentValidation;

namespace ASP_NET_17._TaskFlow_Ownership.Validators;

public class UpdateProjectValidator : AbstractValidator<ProjectUpdateRequest>
{
    public UpdateProjectValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Project Name is required")
            .MinimumLength(3).WithMessage("Project Name must be at least 3 characters long");
    }
}
