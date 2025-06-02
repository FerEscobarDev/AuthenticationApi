namespace AuthenticationApi.Application.Interfaces.Services;

public interface ITwoFactorService
{
    string GenerateSecretKey();
    string GenerateQrCodeUri(string email, string secretKey);
    bool ValidateTwoFactorCode(string secretKey, string code);
    string[] GenerateRecoveryCodes(int count = 5);
}
