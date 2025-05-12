using AuthenticationApi.Application.Commands.ResetPassword;
using FluentValidation;

namespace AuthenticationApi.Application.Validators;

public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordValidator()
    {
        RuleFor(resetPasswordCommand => resetPasswordCommand.Token)
            .NotEmpty().WithMessage("Reset token is required.");

        RuleFor(resetPasswordCommand => resetPasswordCommand.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches("[A-Z]").WithMessage("Must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Must contain at least one number.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Must contain at least one special character.");

        RuleFor(resetPasswordCommand => resetPasswordCommand.ConfirmPassword)
            .Equal(resetPasswordCommand => resetPasswordCommand.NewPassword)
            .WithMessage("Passwords do not match.");
    }
}