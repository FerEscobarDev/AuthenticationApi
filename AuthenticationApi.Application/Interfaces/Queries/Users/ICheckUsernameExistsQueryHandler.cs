using AuthenticationApi.Application.Queries.Users;

namespace AuthenticationApi.Application.Interfaces.Queries.Users;

public interface ICheckUsernameExistsQueryHandler
{
    Task<bool> HandleAsync(CheckUsernameExistsQuery query, CancellationToken cancellationToken = default);   
}