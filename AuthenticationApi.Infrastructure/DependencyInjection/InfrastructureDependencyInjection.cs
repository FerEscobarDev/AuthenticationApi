using AuthenticationApi.Application.Interfaces.Services;
using AuthenticationApi.Application.Interfaces.Persistence;
using AuthenticationApi.Application.Interfaces.Repository;
using AuthenticationApi.Infrastructure.Persistence.Context;
using AuthenticationApi.Infrastructure.Repositories;
using AuthenticationApi.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthenticationApi.Infrastructure.DependencyInjection;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                   .UseSnakeCaseNamingConvention());

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IAuthEmailSender, AuthEmailSender>(); 
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
