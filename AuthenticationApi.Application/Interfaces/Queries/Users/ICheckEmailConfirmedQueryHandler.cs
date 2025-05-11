using AuthenticationApi.Application.Queries.Users;

namespace AuthenticationApi.Application.Interfaces.Queries.Users;

public interface ICheckEmailConfirmedQueryHandler
{
    Task<bool> HandleAsync(CheckEmailConfirmedQuery query, CancellationToken cancellationToken = default);
}