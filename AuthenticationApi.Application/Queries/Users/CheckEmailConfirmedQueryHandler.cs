using AuthenticationApi.Application.Interfaces.Queries.Users;
using AuthenticationApi.Application.Interfaces.Repository;

namespace AuthenticationApi.Application.Queries.Users;

public class CheckEmailConfirmedQueryHandler : ICheckEmailConfirmedQueryHandler
{
    private readonly IUserRepository _userRepository;

    public CheckEmailConfirmedQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> HandleAsync(CheckEmailConfirmedQuery query, CancellationToken cancellationToken = default)
    {
        return await _userRepository.IsEmailConfirmedAsync(query.Email, cancellationToken);
    }
}