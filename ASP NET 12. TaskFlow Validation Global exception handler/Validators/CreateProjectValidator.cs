using ASP_NET_12._TaskFlow_Validation_Global_exception_handler.DTOs.Project_DTOs;
using FluentValidation;

namespace ASP_NET_12._TaskFlow_Validation_Global_exception_handler.Validators;

public class CreateProjectValidator : AbstractValidator<ProjectCreateRequest>
{
    public CreateProjectValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Project Name is required")
            .MinimumLength(3).WithMessage("Project Name must be at least 3 characters long");
    }
}
