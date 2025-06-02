using System.Text.Json;
using AuthenticationApi.Application.Interfaces.Repository;

namespace AuthenticationApi.Application.Commands.ManageTwoFactor;

public class GetRecoveryCodesCommandHandler
{
    private readonly IUserRepository _userRepository;

    public GetRecoveryCodesCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<string>> HandleAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null || string.IsNullOrWhiteSpace(user.TwoFactorRecoveryCodes))
            throw new ApplicationException("No recovery codes found.");

        return JsonSerializer.Deserialize<List<string>>(user.TwoFactorRecoveryCodes!)!;
    }
}