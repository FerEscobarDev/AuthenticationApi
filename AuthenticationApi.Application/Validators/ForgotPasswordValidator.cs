using AuthenticationApi.Application.Commands.ForgotPassword;
using AuthenticationApi.Application.Interfaces.Queries.Users;
using AuthenticationApi.Application.Queries.Users;
using FluentValidation;

namespace AuthenticationApi.Application.Validators;

public class ForgotPasswordValidator: AbstractValidator<ForgotPasswordCommand>
{
    private readonly ICheckEmailConfirmedQueryHandler _checkEmailConfirmedHandler;

    public ForgotPasswordValidator(ICheckEmailConfirmedQueryHandler checkEmailConfirmedHandler)
    {
        _checkEmailConfirmedHandler = checkEmailConfirmedHandler;

        RuleFor(forgotPasswordCommand => forgotPasswordCommand.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MustAsync(EmailIsConfirmed).WithMessage("Email is not confirmed or does not exist.");
    }

    private async Task<bool> EmailIsConfirmed(string email, CancellationToken cancellationToken)
    {
        return await _checkEmailConfirmedHandler.HandleAsync(new CheckEmailConfirmedQuery { EmailOrUsername = email }, cancellationToken);
    }
}