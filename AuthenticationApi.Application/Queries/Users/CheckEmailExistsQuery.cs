namespace AuthenticationApi.Application.Queries.Users;

public class CheckEmailExistsQuery
{
    public string Email { get; set; } = default!;
}