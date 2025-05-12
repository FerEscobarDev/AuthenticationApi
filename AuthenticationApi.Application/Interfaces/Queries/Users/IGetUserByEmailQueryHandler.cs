using AuthenticationApi.Application.Queries.Users;
using AuthenticationApi.Domain.Entities;

namespace AuthenticationApi.Application.Interfaces.Queries.Users;

public interface IGetUserByEmailQueryHandler
{
    Task<User?> HandleAsync(GetUserByEmailQuery query, CancellationToken cancellationToken = default);
}