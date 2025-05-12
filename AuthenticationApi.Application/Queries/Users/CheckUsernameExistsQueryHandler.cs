using AuthenticationApi.Application.Interfaces.Queries.Users;
using AuthenticationApi.Application.Interfaces.Repository;

namespace AuthenticationApi.Application.Queries.Users;

public class CheckUsernameExistsQueryHandler : ICheckUsernameExistsQueryHandler
{
    private readonly IUserRepository _userRepository;
    
    public CheckUsernameExistsQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public Task<bool> HandleAsync(CheckUsernameExistsQuery query, CancellationToken cancellationToken)
    {
        return _userRepository.ExistsByEmailOrUsernameAsync(query.Username, cancellationToken);
    }
}