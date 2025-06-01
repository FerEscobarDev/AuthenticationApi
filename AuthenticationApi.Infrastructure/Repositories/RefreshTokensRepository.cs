using AuthenticationApi.Application.Interfaces.Repository;
using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationApi.Infrastructure.Repositories;

public class RefreshTokensRepository : IRefreshTokensRepository
{
    private readonly ApplicationDbContext _context;
    
    public RefreshTokensRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<RefreshToken?> GetValidTokenWithUserAsync(string refreshTokenHash, CancellationToken cancellationToken = default)
    {
        return await _context.RefreshTokens
            .Include(rt => rt.User)
            .Where(rt => rt.TokenHash == refreshTokenHash && !rt.Revoked)
            .OrderByDescending(rt => rt.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<RefreshToken?> GetValidByHashAsync(string refreshTokenHash, CancellationToken cancellationToken = default)
    {
        return await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.TokenHash == refreshTokenHash && !rt.Revoked, cancellationToken);
    }

    public async Task<List<RefreshToken>> GetRefreshTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.RefreshTokens.Where(refreshToken => refreshToken.UserId == userId).ToListAsync(cancellationToken);
    }
    
    public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        await _context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
    }
    
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}