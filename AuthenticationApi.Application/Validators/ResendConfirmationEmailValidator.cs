using AuthenticationApi.Application.Commands.ConfirmEmail;
using AuthenticationApi.Application.Interfaces.Queries.Users;
using AuthenticationApi.Application.Queries.Users;
using FluentValidation;

namespace AuthenticationApi.Application.Validators;

public class ResendConfirmationEmailValidator : AbstractValidator<ResendConfirmationEmailCommand>
{
    private readonly ICheckUserExistsQueryHandler _checkUserExistsQueryHandler;
    private readonly ICheckEmailConfirmedQueryHandler _checkEmailConfirmedQueryHandler;
    
    public ResendConfirmationEmailValidator(ICheckUserExistsQueryHandler checkUserExistsQueryHandler, ICheckEmailConfirmedQueryHandler checkEmailConfirmedQueryHandler)
    {
        _checkUserExistsQueryHandler = checkUserExistsQueryHandler;
        _checkEmailConfirmedQueryHandler = checkEmailConfirmedQueryHandler;

        RuleFor(resendConfirmationEmailcommand => resendConfirmationEmailcommand.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MustAsync(UserExist).WithMessage("Invalid email.") 
            .DependentRules(() => RuleFor(resendConfirmationEmailcommand => resendConfirmationEmailcommand.Email)
                .MustAsync(EmailIsConfirmed).WithMessage("Email is already confirmed."));
    }

    private async Task<bool> UserExist(string email, CancellationToken cancellationToken)
    {
        return await _checkUserExistsQueryHandler.HandleAsync(new CheckUserExistsQuery {EmailOrUsername = email}, cancellationToken);
    }
    
    private async Task<bool> EmailIsConfirmed(string email, CancellationToken cancellationToken)
    {
        return await _checkEmailConfirmedQueryHandler.HandleAsync(new CheckEmailConfirmedQuery { EmailOrUsername = email }, cancellationToken);
    }
}