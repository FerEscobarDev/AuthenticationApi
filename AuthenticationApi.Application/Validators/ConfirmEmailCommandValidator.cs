using AuthenticationApi.Application.Commands.ConfirmEmail;
using FluentValidation;

namespace AuthenticationApi.Application.Validators;

public class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailCommandValidator()
    {
        RuleFor(confirmEmailCommand => confirmEmailCommand.Token )
            .NotEmpty().WithMessage("Token is required.");
    }
}