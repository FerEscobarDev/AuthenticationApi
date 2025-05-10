using AuthenticationApi.Application.Queries.Users;

namespace AuthenticationApi.Application.Interfaces.Queries.Users;

public interface ICheckEmailExistsQueryHandler
{
    Task<bool> HandleAsync(CheckEmailExistsQuery query, CancellationToken cancellationToken = default);
}