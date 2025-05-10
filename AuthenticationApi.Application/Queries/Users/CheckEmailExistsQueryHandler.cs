using AuthenticationApi.Application.Interfaces.Queries.Users;
using AuthenticationApi.Application.Interfaces.Repository;

namespace AuthenticationApi.Application.Queries.Users;

public class CheckEmailExistsQueryHandler : ICheckEmailExistsQueryHandler
{
    private readonly IUserRepository _userRepository;
    
    public CheckEmailExistsQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<bool> HandleAsync(CheckEmailExistsQuery query, CancellationToken cancellationToken)
    {
        return await _userRepository.EmailExistsAsync(query.Email);
    }
}