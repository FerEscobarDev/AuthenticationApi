using AuthenticationApi.Application.Interfaces.Queries.Users;
using AuthenticationApi.Application.Interfaces.Repository;

namespace AuthenticationApi.Application.Queries.Users;

public class CheckUserExistsQueryHandler : ICheckUserExistsQueryHandler
{
    private readonly IUserRepository _userRepository;

    public CheckUserExistsQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> HandleAsync(CheckUserExistsQuery query, CancellationToken cancellationToken = default)
    {
        return await _userRepository.ExistsByEmailOrUsernameAsync(query.EmailOrUsername, cancellationToken);
    }
}