using AuthenticationApi.Domain.Entities;

namespace AuthenticationApi.Application.Interfaces.Repository;

public interface IUserRepository
{
    Task<bool> EmailExistsAsync(string email);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid id);
    Task AddAsync(User user);
}