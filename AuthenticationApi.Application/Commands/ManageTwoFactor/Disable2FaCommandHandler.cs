using AuthenticationApi.Application.Interfaces.Repository;
using OtpNet;

namespace AuthenticationApi.Application.Commands.ManageTwoFactor;

public class Disable2FaCommandHandler
{
    private readonly IUserRepository _userRepository;
 
    public Disable2FaCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task HandleAsync(Guid userId, Disable2FaCommand command, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null || string.IsNullOrWhiteSpace(user.TwoFactorSecretKey))
            throw new ApplicationException("Two-factor authentication is not configured.");

        var totp = new Totp(Base32Encoding.ToBytes(user.TwoFactorSecretKey));
        if (!totp.VerifyTotp(command.Code, out _, new VerificationWindow(2, 2)))
            throw new ApplicationException("Invalid two-factor authentication code.");

        user.TwoFactorEnabled = false;
        user.TwoFactorSecretKey = null;
        user.TwoFactorRecoveryCodes = null;

        await _userRepository.UpdateAsync(user, cancellationToken);
    }
}
