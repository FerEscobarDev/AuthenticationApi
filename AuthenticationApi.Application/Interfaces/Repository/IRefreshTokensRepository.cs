using AuthenticationApi.Domain.Entities;

namespace AuthenticationApi.Application.Interfaces.Repository;

public interface IRefreshTokensRepository
{
    Task<RefreshToken?> GetValidTokenWithUserAsync(string refreshTokenHash, CancellationToken cancellationToken = default);
    Task<RefreshToken?> GetValidByHashAsync(string refreshTokenHash, CancellationToken cancellationToken = default);
    Task<List<RefreshToken>> GetRefreshTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);   
}