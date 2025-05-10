using AuthenticationApi.Application.Commands.RegisterUser;
using AuthenticationApi.Application.Interfaces.Repository;
using FluentValidation;

namespace AuthenticationApi.Application.Validators;

public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    private readonly IUserRepository _userRepository;

    public RegisterUserValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(registerUserCommand => registerUserCommand.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MustAsync(EmailNotExists).WithMessage("Email is already registered.");

        RuleFor(registerUserCommand => registerUserCommand.UserName)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long.");

        RuleFor(registerUserCommand => registerUserCommand.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one number.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");
    }

    private async Task<bool> EmailNotExists(string email, CancellationToken cancellationToken)
    {
        return !await _userRepository.EmailExistsAsync(email);
    }
}