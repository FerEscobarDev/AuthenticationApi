using AuthenticationApi.Application.Commands.RefreshToken;
using FluentValidation;

namespace AuthenticationApi.Application.Validators;

public class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenValidator()
    {
        RuleFor(refreshTokenCommand => refreshTokenCommand.RefreshToken )
            .NotEmpty().WithMessage("Refresh token is required.");
    }
}