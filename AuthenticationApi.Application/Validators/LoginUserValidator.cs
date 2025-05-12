using AuthenticationApi.Application.Commands.LoginUser;
using AuthenticationApi.Application.Interfaces.Queries.Users;
using AuthenticationApi.Application.Queries.Users;
using FluentValidation;

namespace AuthenticationApi.Application.Validators;

public class LoginUserValidator : AbstractValidator<LoginUserCommand>
{
    private readonly ICheckUserExistsQueryHandler _userExistsQueryHandler;
    private readonly ICheckEmailConfirmedQueryHandler _checkEmailConfirmedQueryHandler;
    
    public LoginUserValidator(ICheckUserExistsQueryHandler userExistsQueryHandler, ICheckEmailConfirmedQueryHandler checkEmailConfirmedQueryHandler)
    {
        _userExistsQueryHandler = userExistsQueryHandler;
        _checkEmailConfirmedQueryHandler = checkEmailConfirmedQueryHandler;

        RuleFor(loginUserCommand => loginUserCommand.EmailOrUsername)
            .NotEmpty().WithMessage("Email or username is required.")
            .MustAsync(UserExists).WithMessage("Invalid credentials.")
            .DependentRules(() => RuleFor(loginUserCommand => loginUserCommand.EmailOrUsername)
                .MustAsync(EmailIsConfirmed).WithMessage("Email is not confirmed or does not exist."));       

        RuleFor(loginUserCommand => loginUserCommand.Password)
            .NotEmpty().WithMessage("Password is required.");
    }

    private async Task<bool> UserExists(string emailOrUserName, CancellationToken cancellationToken)
    {
        return await _userExistsQueryHandler.HandleAsync(new CheckUserExistsQuery { EmailOrUsername = emailOrUserName }, cancellationToken);
    }
    
    private async Task<bool> EmailIsConfirmed(string emailOrUsername, CancellationToken cancellationToken)
    {
        return await _checkEmailConfirmedQueryHandler.HandleAsync(new CheckEmailConfirmedQuery { EmailOrUsername = emailOrUsername }, cancellationToken);
    }
}