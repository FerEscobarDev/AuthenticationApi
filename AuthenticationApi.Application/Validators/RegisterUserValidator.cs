using AuthenticationApi.Application.Commands.RegisterUser;
using AuthenticationApi.Application.Interfaces.Queries.Users;
using AuthenticationApi.Application.Queries.Users;
using FluentValidation;
using FluentValidation.Validators;

namespace AuthenticationApi.Application.Validators;

public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    private readonly ICheckEmailExistsQueryHandler _emailExistsHandler;
    private readonly ICheckUsernameExistsQueryHandler _usernameExistsQueryHandler;

    public RegisterUserValidator(ICheckEmailExistsQueryHandler  emailExistsHandler, ICheckUsernameExistsQueryHandler usernameExistsQueryHandler)
    {
        _emailExistsHandler = emailExistsHandler;
        _usernameExistsQueryHandler = usernameExistsQueryHandler;
        
        RuleFor(registerUserCommand => registerUserCommand.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .Length(3, 250).WithMessage("Username must be between 3 and 250 characters.")
            .Matches(@"^[A-Za-zÁÉÍÓÚáéíóúñÑ\s]+$").WithMessage("First name can only contain letters.");

        RuleFor(registerUserCommand => registerUserCommand.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MustAsync(EmailNotExists).WithMessage("Email is already registered.");

        RuleFor(registerUserCommand => registerUserCommand.UserName)
            .NotEmpty().WithMessage("Username is required.")
            .Length(3, 20).WithMessage("Username must be between 3 and 20 characters.")
            .Matches(@"^[a-zA-Z0-9_-]+$").WithMessage("Username can only contain letters, numbers, underscores, and hyphens.")
            .Must(username => !char.IsDigit(username.First())).WithMessage("Username cannot start with a number.")
            .Must(username => !username.StartsWith("-") && !username.StartsWith("_"))
            .WithMessage("Username cannot start with a symbol.")
            .MustAsync(UsernameNotExists).WithMessage("Username is not available.");       

        RuleFor(registerUserCommand => registerUserCommand.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one number.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");
        
        RuleFor(registerUserCommand => registerUserCommand.ConfirmPassword)
            .Equal(registerUserCommand => registerUserCommand.Password)
            .WithMessage("Passwords do not match.");

    }

    private async Task<bool> EmailNotExists(string email, CancellationToken cancellationToken)
    {
        return !await _emailExistsHandler.HandleAsync(new CheckEmailExistsQuery { Email = email }, cancellationToken);
    }

    private async Task<bool> UsernameNotExists(string username, CancellationToken cancellation)
    {
        return !await _usernameExistsQueryHandler.HandleAsync(new CheckUsernameExistsQuery { Username = username }, cancellation);       
    }
}