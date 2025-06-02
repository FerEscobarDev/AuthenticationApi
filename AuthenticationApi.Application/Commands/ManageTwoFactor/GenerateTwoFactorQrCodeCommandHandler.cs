using AuthenticationApi.Application.DTOs.Auth;
using AuthenticationApi.Application.Interfaces.Repository;
using AuthenticationApi.Application.Interfaces.Services;

namespace AuthenticationApi.Application.Commands.ManageTwoFactor;

public class GenerateTwoFactorQrCodeCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly ITwoFactorService _twoFactorService;
    
    public GenerateTwoFactorQrCodeCommandHandler(IUserRepository userRepository, ITwoFactorService twoFactorService)
    {
        _userRepository = userRepository;
        _twoFactorService = twoFactorService;
    }

    public async Task<GenerateTwoFactorQrCodeDto> HandleAsync(GenerateTwoFactorQrCodeCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
            throw new ApplicationException("User not found");
        
        if(!string.IsNullOrWhiteSpace(user.TwoFactorSecretKey))
            return new GenerateTwoFactorQrCodeDto(user.TwoFactorSecretKey, _twoFactorService.GenerateQrCodeUri(user.Email, user.TwoFactorSecretKey));

        var secret = _twoFactorService.GenerateSecretKey();
        user.TwoFactorSecretKey = secret;

        await _userRepository.UpdateAsync(user, cancellationToken);
        
        var uri = _twoFactorService.GenerateQrCodeUri(user.Email, secret);
        return new GenerateTwoFactorQrCodeDto(secret, uri);
    }
}