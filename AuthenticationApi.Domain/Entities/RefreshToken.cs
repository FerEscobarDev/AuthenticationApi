namespace AuthenticationApi.Domain.Entities
{
    public sealed class RefreshToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string TokenHash { get; set; } = default!;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool Revoked { get; set; } = false;

        // Relación
        public Guid UserId { get; set; }
        public User User { get; set; } = default!;
    }
}
