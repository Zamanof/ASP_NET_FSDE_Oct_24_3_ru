using ASP_NET_15._TaskFlow_Refresh_Token.DTOs.Auth_DTOs;
using FluentValidation;

namespace ASP_NET_15._TaskFlow_Refresh_Token.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("FirstName required")
            .MinimumLength(2).WithMessage("User first name must be at least 2 characters long");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("LastName required")
            .MinimumLength(2).WithMessage("User last name must be at least 2 characters long");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email required")
            .EmailAddress().WithMessage("Email is not valid");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)").WithMessage("Passwords must be at least 6 characters.,Passwords must have at least one digit ('0'-'9').,Passwords must have at least one lowercase ('a'-'z').,Passwords must have at least one uppercase ('A'-'Z')");

        RuleFor(x => x.ConfirmedPassword)
            .NotEmpty().WithMessage("Confirmed Password required")
            .Equal(x => x.Password).WithMessage("Passwords do not match");
    }
}

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email required")
            .EmailAddress().WithMessage("Email is not valid");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)").WithMessage("Passwords must be at least 6 characters.,Passwords must have at least one digit ('0'-'9').,Passwords must have at least one lowercase ('a'-'z').,Passwords must have at least one uppercase ('A'-'Z')");
    }
}

public class RefreshRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("RefreshToken required");
    }
}
