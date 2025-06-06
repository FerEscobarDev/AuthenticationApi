﻿namespace AuthenticationApi.Application.Commands.RegisterUser
{
    public sealed class RegisterUserCommand
    {
        public string FirstName { get; init; } = default!;
        public string LastName { get; init; } = default!;
        public string UserName { get; init; } = default!;
        public string Email { get; init; } = default!;
        public string Password { get; init; } = default!;
        public string ConfirmPassword { get; init; } = default!;
    }
}
