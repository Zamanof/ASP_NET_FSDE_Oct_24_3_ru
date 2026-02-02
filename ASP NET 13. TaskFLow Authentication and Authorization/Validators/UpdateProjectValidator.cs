using ASP_NET_13._TaskFLow_Authentication_and_Authorization.DTOs.Project_DTOs;
using FluentValidation;

namespace ASP_NET_13._TaskFLow_Authentication_and_Authorization.Validators;

public class UpdateProjectValidator : AbstractValidator<ProjectUpdateRequest>
{
    public UpdateProjectValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Project Name is required")
            .MinimumLength(3).WithMessage("Project Name must be at least 3 characters long");
    }
}
