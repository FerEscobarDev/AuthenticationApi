using AuthenticationApi.Application.Queries.Users;
using AuthenticationApi.Domain.Entities;

namespace AuthenticationApi.Application.Interfaces.Queries.Users;

public interface IGetUserByEmailOrUsernameQueryHandler
{
    Task<User?> HandleAsync(GetUserByEmailOrUserNameQuery query, CancellationToken cancellationToken = default);
}