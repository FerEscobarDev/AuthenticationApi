using AuthenticationApi.Application.Interfaces.Queries.Users;
using AuthenticationApi.Application.Interfaces.Repository;
using AuthenticationApi.Domain.Entities;

namespace AuthenticationApi.Application.Queries.Users;

public class GetUserByEmailQueryHandler : IGetUserByEmailQueryHandler
{
    private readonly IUserRepository _userRepository;

    public GetUserByEmailQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;       
    }
    public Task<User?> HandleAsync(GetUserByEmailQuery query, CancellationToken cancellationToken = default)
    {
        return  _userRepository.GetByEmailAsync(query.Email, cancellationToken);
    }   
}