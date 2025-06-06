using AuthenticationApi.Application.Interfaces.Repository;
using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationApi.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    
    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AnyAsync(user => user.Email.ToLower() == email.ToLower(), cancellationToken);
    }

    public async Task<User?> GetByEmailOrUsernameAsync(string emailOrUsername, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(user => user.Email.ToLower() == emailOrUsername.ToLower() || user.UserName == emailOrUsername, cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FindAsync(id, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }
    
    public async Task<bool> ExistsByEmailOrUsernameAsync(string emailOrUsername, CancellationToken cancellationToken = default)
    {
        return await _context.Users.AnyAsync(user =>
            user.Email.ToLower() == emailOrUsername.ToLower() ||
            user.UserName.ToLower() == emailOrUsername.ToLower(), cancellationToken);
    }
    
    public async Task<bool> IsEmailConfirmedAsync(string emailOrUsername, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AnyAsync(user => (user.Email == emailOrUsername || user.UserName == emailOrUsername) && user.EmailConfirmed, cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task EnableTwoFactorAsync(User user, string secretKey, string[] recoveryCodes, CancellationToken cancellationToken = default)
    {
        user.TwoFactorEnabled = true;
        user.TwoFactorSecretKey = secretKey;
        user.TwoFactorRecoveryCodes = string.Join(",", recoveryCodes);
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DisableTwoFactorAsync(User user, CancellationToken cancellationToken = default)
    {
        user.TwoFactorEnabled = false;
        user.TwoFactorSecretKey = null;
        user.TwoFactorRecoveryCodes = null;
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}