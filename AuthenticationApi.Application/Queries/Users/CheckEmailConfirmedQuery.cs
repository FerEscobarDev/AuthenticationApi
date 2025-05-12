namespace AuthenticationApi.Application.Queries.Users;

public class CheckEmailConfirmedQuery
{
    public string EmailOrUsername { get; set; } = default!;
}