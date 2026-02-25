using ASP_NET_21._TaskFlow.BLL.DTOs;
using FluentValidation;

namespace ASP_NET_21._TaskFlow.API.Validators;

public class RegisterValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Firstname is required")
            .MinimumLength(2).WithMessage("Firstname must be at least 2 characters long");

        RuleFor(x => x.LastName)
           .NotEmpty().WithMessage("Lastname is required")
           .MinimumLength(2).WithMessage("Lastname must be at least 2 characters long");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Lastname is required")
            .EmailAddress().WithMessage("Email is not valid");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long")
            .Password();

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Confirmed password is required")
            .Equal(x => x.Password).WithMessage("Passwords do not match");

    }
}

public class LoginValidator : AbstractValidator<LoginRequestDto>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Lastname is required")
            .EmailAddress().WithMessage("Email is not valid");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long")
            .Password();
    }
}

public class RefreshValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh Token is required");
    }
}