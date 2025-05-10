using AuthenticationApi.Application.Commands.LoginUser;
using AuthenticationApi.Application.Interfaces.Queries.Users;
using AuthenticationApi.Application.Queries.Users;
using FluentValidation;

namespace AuthenticationApi.Application.Validators;

public class LoginUserValidator : AbstractValidator<LoginUserCommand>
{
    private readonly ICheckUserExistsQueryHandler _userExistsQueryHandler;
    
    public LoginUserValidator(ICheckUserExistsQueryHandler userExistsQueryHandler)
    {
        _userExistsQueryHandler = userExistsQueryHandler;
        
        RuleFor(x => x.EmailOrUsername)
            .NotEmpty().WithMessage("Email or username is required.")
            .MustAsync(UserExists).WithMessage("Invalid email or password.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }

    private async Task<bool> UserExists(string input, CancellationToken ct)
    {
        return await _userExistsQueryHandler.HandleAsync(new CheckUserExistsQuery { EmailOrUsername = input }, ct);
    }
}