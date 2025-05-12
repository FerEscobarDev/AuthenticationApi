using AuthenticationApi.Application.Commands.Logout;
using FluentValidation;

namespace AuthenticationApi.Application.Validators;

public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(command => command.RefreshToken )
            .NotEmpty().WithMessage("Refresh token is required.");
    }
}