namespace AuthenticationApi.Application.Queries.Users;

public class CheckUserExistsQuery
{
    public string EmailOrUsername { get; set; } = default!;
}