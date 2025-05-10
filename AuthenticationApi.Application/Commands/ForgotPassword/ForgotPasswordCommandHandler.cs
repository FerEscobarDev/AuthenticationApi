using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Application.Interfaces.Persistence;
using AuthenticationApi.Application.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AuthenticationApi.Application.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler
{
    private readonly IApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IAuthService _authService;
    private readonly IAuthEmailSender _authEmailSender;

    public ForgotPasswordCommandHandler(
        IApplicationDbContext context,
        IConfiguration configuration,
        IAuthService authService,
        IAuthEmailSender authEmailSender)
    {
        _context = context;
        _configuration = configuration;
        _authService = authService;
        _authEmailSender = authEmailSender;
    }

    public async Task HandleAsync(ForgotPasswordCommand command)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == command.Email);

        if (user is null)
            throw new ApplicationException("User not found.");

        if (!user.EmailConfirmed)
            throw new ApplicationException("You must confirm your email before resetting your password.");

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