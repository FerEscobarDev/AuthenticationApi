namespace AuthenticationApi.Application.DTOs.Auth
{
    public sealed class AuthResultDto
    {
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
        public string TokenType { get; set; } = "Bearer";
        public int ExpiresIn { get; set; } 
    }
}
