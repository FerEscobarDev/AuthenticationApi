using AuthenticationApi.Application.Commands.ConfirmEmail;
using AuthenticationApi.Application.Commands.LoginUser;
using AuthenticationApi.Application.Commands.Logout;
using AuthenticationApi.Application.Commands.RefreshToken;
using AuthenticationApi.Application.Commands.RegisterUser;
using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Infrastructure.DependencyInjection;
using AuthenticationApi.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AuthenticationApi.Application.Commands.ForgotPassword;
using AuthenticationApi.Application.Commands.ResetPassword;
using AuthenticationApi.Application.Interfaces.Queries.Users;
using AuthenticationApi.Application.Queries.Users;
using AuthenticationApi.Application.Validators;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//{
//    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
//           .UseSnakeCaseNamingConvention();
//});

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!))
        };
    });

builder.Services.AddScoped<IValidator<RegisterUserCommand>, RegisterUserValidator>();
//builder.Services.AddScoped<IValidator<ResetPasswordCommand>, ResetPasswordValidator>();
//builder.Services.AddScoped<IValidator<LoginUserCommand>, LoginUserValidator>();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<RegisterUserCommandHandler>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<LoginUserCommandHandler>();
builder.Services.AddScoped<RefreshTokenCommandHandler>();
builder.Services.AddScoped<LogoutCommandHandler>();
builder.Services.AddScoped<ConfirmEmailCommandHandler>();
builder.Services.AddScoped<ResendConfirmationEmailCommandHandler>();
builder.Services.AddScoped<ForgotPasswordCommandHandler>();
builder.Services.AddScoped<ResetPasswordCommandHandler>();
builder.Services.AddScoped<ICheckEmailExistsQueryHandler, CheckEmailExistsQueryHandler>();
builder.Services.AddScoped<ICheckUserExistsQueryHandler, CheckUserExistsQueryHandler>();
builder.Services.AddScoped<ICheckEmailConfirmedQueryHandler, CheckEmailConfirmedQueryHandler>();
builder.Services.AddScoped<IValidator<LoginUserCommand>, LoginUserValidator>();
builder.Services.AddScoped<IValidator<ForgotPasswordCommand>, ForgotPasswordValidator>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
