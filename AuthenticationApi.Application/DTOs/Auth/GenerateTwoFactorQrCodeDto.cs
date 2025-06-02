namespace AuthenticationApi.Application.DTOs.Auth;

public record GenerateTwoFactorQrCodeDto(string SecretKey, string QrCodeUri);
