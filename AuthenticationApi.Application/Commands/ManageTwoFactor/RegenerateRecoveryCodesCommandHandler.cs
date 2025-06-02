using System.Text.Json;
using AuthenticationApi.Application.Interfaces.Repository;
using AuthenticationApi.Application.Interfaces.Services;

namespace AuthenticationApi.Application.Commands.ManageTwoFactor;

public class RegenerateRecoveryCodesCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly ITwoFactorService _twoFactorService;

    public RegenerateRecoveryCodesCommandHandler(
        IUserRepository userRepository,
        ITwoFactorService twoFactorService)
    {
        _userRepository = userRepository;
        _twoFactorService = twoFactorService;
    }

    public async Task<IEnumerable<string>> HandleAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null || !user.TwoFactorEnabled)
            throw new ApplicationException("Two-factor authentication must be enabled to generate recovery codes.");

        var recoveryCodes = _twoFactorService.GenerateRecoveryCodes(10);
        user.TwoFactorRecoveryCodes = JsonSerializer.Serialize(recoveryCodes);

        await _userRepository.UpdateAsync(user, cancellationToken);

        return recoveryCodes;
    }
}