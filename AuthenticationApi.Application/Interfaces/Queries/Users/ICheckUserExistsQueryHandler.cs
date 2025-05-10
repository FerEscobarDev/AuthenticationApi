using AuthenticationApi.Application.Queries.Users;

namespace AuthenticationApi.Application.Interfaces.Queries.Users;

public interface ICheckUserExistsQueryHandler
{
    Task<bool> HandleAsync(CheckUserExistsQuery query, CancellationToken cancellationToken = default);
}