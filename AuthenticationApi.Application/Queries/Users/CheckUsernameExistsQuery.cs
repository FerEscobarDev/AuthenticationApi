namespace AuthenticationApi.Application.Queries.Users;

public class CheckUsernameExistsQuery
{
    public string Username { get; set; } = default!;   
}