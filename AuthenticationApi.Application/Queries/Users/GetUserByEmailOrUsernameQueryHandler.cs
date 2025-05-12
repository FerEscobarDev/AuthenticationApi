using AuthenticationApi.Application.Interfaces.Queries.Users;
using AuthenticationApi.Application.Interfaces.Repository;
using AuthenticationApi.Domain.Entities;

namespace AuthenticationApi.Application.Queries.Users;

public class GetUserByEmailOrUsernameQueryHandler : IGetUserByEmailOrUsernameQueryHandler
{
    private readonly IUserRepository _userRepository;

    public GetUserByEmailOrUsernameQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;       
    }
    public Task<User?> HandleAsync(GetUserByEmailOrUserNameQuery query, CancellationToken cancellationToken = default)
    {
        return  _userRepository.GetByEmailOrUsernameAsync(query.EmailOrUserName, cancellationToken);
    }   
}