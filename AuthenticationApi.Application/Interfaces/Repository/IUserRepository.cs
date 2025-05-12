using AuthenticationApi.Domain.Entities;

namespace AuthenticationApi.Application.Interfaces.Repository;

public interface IUserRepository
{
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailOrUsernameAsync(string emailOrUsername, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailOrUsernameAsync(string emailOrUsername, CancellationToken cancellationToken = default);
    Task<bool> IsEmailConfirmedAsync(string emailOrUsername, CancellationToken cancellationToken = default);

}