using System.Web;
using AuthenticationApi.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using OtpNet;

namespace AuthenticationApi.Infrastructure.Services;

public class TwoFactorService : ITwoFactorService
{
    private readonly IConfiguration _configuration;
    
    public TwoFactorService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public string GenerateSecretKey()
    {
        var key = KeyGeneration.GenerateRandomKey(20);
        return Base32Encoding.ToString(key);
    }

    public string GenerateQrCodeUri(string email, string secretKey)
    {
        var issuer = _configuration["AppName"] ?? "AuthenticationApi";
        var encodedEmail = HttpUtility.UrlEncode(email);
        var encodedIssuer = HttpUtility.UrlEncode(issuer);
        
        return $"otpauth://totp/{encodedIssuer}:{encodedEmail}?secret={secretKey}&issuer={encodedIssuer}&digits=6";  
    }

    public bool ValidateTwoFactorCode(string secretKey, string code)
    {
        var totp = new Totp(Base32Encoding.ToBytes(secretKey));
        return totp.VerifyTotp(code, out _, VerificationWindow.RfcSpecifiedNetworkDelay);
    }

    public string[] GenerateRecoveryCodes(int count = 5)
    {
        return Enumerable.Range(0, count)
            .Select(_ => Guid.NewGuid().ToString("N")[..8].ToUpper())
            .ToArray();   
    }
}