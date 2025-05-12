using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Application.Interfaces.Persistence;
using AuthenticationApi.Application.Interfaces.Queries.Users;
using AuthenticationApi.Application.Interfaces.Repository;
using AuthenticationApi.Application.Interfaces.Services;
using AuthenticationApi.Application.Queries.Users;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AuthenticationApi.Application.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler
{
    private readonly IConfiguration _configuration;
    private readonly IAuthService _authService;
    private readonly IAuthEmailSender _authEmailSender;
    private readonly IValidator<ForgotPasswordCommand> _validator;
    private readonly IGetUserByEmailQueryHandler _getUserByEmailQueryHandler;

    public ForgotPasswordCommandHandler(
        IConfiguration configuration,
        IAuthService authService,
        IAuthEmailSender authEmailSender,
        IValidator<ForgotPasswordCommand> validator,
        IGetUserByEmailQueryHandler getUserByEmailQueryHandler)
    {
        _configuration = configuration;
        _authService = authService;
        _authEmailSender = authEmailSender;
        _validator = validator;
        _getUserByEmailQueryHandler = getUserByEmailQueryHandler;
    }

    public async Task HandleAsync(ForgotPasswordCommand command, CancellationToken cancellationToken = default)
    {
        var validated = await _validator.ValidateAsync(command, cancellationToken);
        if (!validated.IsValid)
            throw new ValidationException(validated.Errors);

        var user = await _getUserByEmailQueryHandler.HandleAsync(new GetUserByEmailQuery { Email = command.Email }, cancellationToken);

        if (user is null)
            throw new ApplicationException("Error getting user.");
        
        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName
        };

        var token = _authService.GeneratePasswordResetToken(userDto);
        var resetLink = $"{_configuration["Frontend:BaseUrl"]}/reset-password?token={token}";

        await _authEmailSender.SendPasswordResetLinkAsync(userDto, user.Email, resetLink);
    }
}