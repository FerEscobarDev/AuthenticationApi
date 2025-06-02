using OtpNet;
using System.Text.Json;
using AuthenticationApi.Application.Interfaces.Repository;
using AuthenticationApi.Application.Interfaces.Services;

namespace AuthenticationApi.Application.Commands.ManageTwoFactor;

public class Enable2FaCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly ITwoFactorService _twoFactorService;

    public Enable2FaCommandHandler(IUserRepository userRepository, ITwoFactorService twoFactorService)
    {
        _userRepository = userRepository;
        _twoFactorService = twoFactorService;
    }

    public async Task HandleAsync(Enable2FaCommand command, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(Guid.Parse(command.UserId), cancellationToken);
        if (user is null)
            throw new ApplicationException("User not found.");

        if (string.IsNullOrWhiteSpace(user.TwoFactorSecretKey))
            throw new ApplicationException("2FA is not initialized for this user.");

        var totp = new Totp(Base32Encoding.ToBytes(user.TwoFactorSecretKey));
        
        if (!totp.VerifyTotp(command.Code, out _, new VerificationWindow(2, 2)))
            throw new ApplicationException("Invalid two-factor authentication code.");

        user.TwoFactorEnabled = true;
        user.TwoFactorRecoveryCodes = JsonSerializer.Serialize(_twoFactorService.GenerateRecoveryCodes(8));

        await _userRepository.UpdateAsync(user, cancellationToken);
    }
}