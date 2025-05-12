namespace AuthenticationApi.Application.Queries.Users;

public class GetUserByEmailOrUserNameQuery
{
    public string EmailOrUserName { get; set; } = default!;  
}