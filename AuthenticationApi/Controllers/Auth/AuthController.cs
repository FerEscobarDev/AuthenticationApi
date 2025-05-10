using AuthenticationApi.Application.Commands.ConfirmEmail;
using AuthenticationApi.Application.Commands.ForgotPassword;
using AuthenticationApi.Application.Commands.LoginUser;
using AuthenticationApi.Application.Commands.Logout;
using AuthenticationApi.Application.Commands.RefreshToken;
using AuthenticationApi.Application.Commands.RegisterUser;
using AuthenticationApi.Application.Commands.ResetPassword;
using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.DTOs.Auth;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationApi.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly RegisterUserCommandHandler _registerHandler;
        private readonly LoginUserCommandHandler _loginHandler;
        private readonly RefreshTokenCommandHandler _refreshTokenHandler;
        private readonly LogoutCommandHandler _logoutHandler;
        private readonly ConfirmEmailCommandHandler _confirmEmailHandler;
        private readonly ResendConfirmationEmailCommandHandler _resendConfirmationEmailHandler;
        private readonly ForgotPasswordCommandHandler _forgotPasswordHandler;
        private readonly ResetPasswordCommandHandler _resetPasswordHandler;

        public AuthController(
            RegisterUserCommandHandler registerHandler, 
            LoginUserCommandHandler loginHandler,
            RefreshTokenCommandHandler refreshTokenHandler,
            LogoutCommandHandler logoutHandler,
            ConfirmEmailCommandHandler confirmEmailHandler,
            ResendConfirmationEmailCommandHandler resendConfirmationEmailHandler,
            ForgotPasswordCommandHandler forgotPasswordHandler,
            ResetPasswordCommandHandler resetPasswordHandler)
        {
            _registerHandler = registerHandler;
            _loginHandler = loginHandler;
            _refreshTokenHandler = refreshTokenHandler;
            _logoutHandler = logoutHandler;
            _confirmEmailHandler = confirmEmailHandler;
            _resendConfirmationEmailHandler = resendConfirmationEmailHandler;
            _forgotPasswordHandler = forgotPasswordHandler;
            _resetPasswordHandler = resetPasswordHandler;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterUserCommand command)
        {
            try
            {
                var result = await _registerHandler.HandleAsync(command);
                return CreatedAtAction(nameof(Register), new { id = result.Id }, result);
                //return CreatedAtAction(
                //    actionName: nameof(UsersController.GetById),
                //    controllerName: "Users",
                //    routeValues: new { id = result.Id },
                //    value: result
                //);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new
                {
                    errors = ex.Errors.Select(e => e.ErrorMessage)
                });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResultDto>> Login([FromBody] LoginUserCommand command)
        {
            try
            {
                var result = await _loginHandler.HandleAsync(command);
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResultDto>> Refresh([FromBody] RefreshTokenCommand command)
        {
            try
            {
                var result = await _refreshTokenHandler.HandleAsync(command);
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutCommand command)
        {
            try
            {
                await _logoutHandler.HandleAsync(command);
                return Ok(new { message = "Refresh token successfully revoked." });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailCommand command)
        {
            try
            {
                await _confirmEmailHandler.HandleAsync(command);
                return Ok(new { message = "Email confirmed successfully." });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("resend-confirmation")]
        public async Task<IActionResult> ResendConfirmation([FromBody] ResendConfirmationEmailCommand command)
        {
            try
            {
                await _resendConfirmationEmailHandler.HandleAsync(command);
                return Ok(new { message = "Confirmation email resent successfully." });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
        {
            try
            {
                await _forgotPasswordHandler.HandleAsync(command);
                return Ok(new { message = "Password reset link has been sent." });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            try
            {
                await _resetPasswordHandler.HandleAsync(command);
                return Ok(new { message = "Password has been reset successfully." });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
