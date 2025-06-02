using AuthenticationApi.Application.Commands.ConfirmEmail;
using AuthenticationApi.Application.Commands.ForgotPassword;
using AuthenticationApi.Application.Commands.LoginUser;
using AuthenticationApi.Application.Commands.Logout;
using AuthenticationApi.Application.Commands.ManageTwoFactor;
using AuthenticationApi.Application.Commands.RefreshToken;
using AuthenticationApi.Application.Commands.RegisterUser;
using AuthenticationApi.Application.Commands.ResetPassword;
using AuthenticationApi.Common.Extensions;
using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.DTOs.Auth;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
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
        private readonly GenerateTwoFactorQrCodeCommandHandler _generateTwoFactorQrCodeHandler;
        private readonly Enable2FaCommandHandler _enable2FaHandler;
        private readonly Disable2FaCommandHandler _disable2FaHandler;
        private readonly RegenerateRecoveryCodesCommandHandler _regenerateRecoveryCodesHandler;
        private readonly GetRecoveryCodesCommandHandler _getRecoveryCodesHandler;

        public AuthController(
            RegisterUserCommandHandler registerHandler, 
            LoginUserCommandHandler loginHandler,
            RefreshTokenCommandHandler refreshTokenHandler,
            LogoutCommandHandler logoutHandler,
            ConfirmEmailCommandHandler confirmEmailHandler,
            ResendConfirmationEmailCommandHandler resendConfirmationEmailHandler,
            ForgotPasswordCommandHandler forgotPasswordHandler,
            ResetPasswordCommandHandler resetPasswordHandler,
            GenerateTwoFactorQrCodeCommandHandler generateTwoFactorQrCodeHandler,
            Enable2FaCommandHandler enable2FaHandler,
            Disable2FaCommandHandler disable2FaHandler,
            RegenerateRecoveryCodesCommandHandler regenerateRecoveryCodesHandler,
            GetRecoveryCodesCommandHandler getRecoveryCodesHandler)
        {
            _registerHandler = registerHandler;
            _loginHandler = loginHandler;
            _refreshTokenHandler = refreshTokenHandler;
            _logoutHandler = logoutHandler;
            _confirmEmailHandler = confirmEmailHandler;
            _resendConfirmationEmailHandler = resendConfirmationEmailHandler;
            _forgotPasswordHandler = forgotPasswordHandler;
            _resetPasswordHandler = resetPasswordHandler;
            _generateTwoFactorQrCodeHandler = generateTwoFactorQrCodeHandler;
            _enable2FaHandler = enable2FaHandler;
            _disable2FaHandler = disable2FaHandler;
            _regenerateRecoveryCodesHandler = regenerateRecoveryCodesHandler;
            _getRecoveryCodesHandler = getRecoveryCodesHandler;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterUserCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _registerHandler.HandleAsync(command, cancellationToken);
                return CreatedAtAction(nameof(Register), new { id = result.Id }, result);
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
        public async Task<ActionResult<AuthResultDto>> Login([FromBody] LoginUserCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _loginHandler.HandleAsync(command, cancellationToken);
                return Ok(result);
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

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResultDto>> Refresh([FromBody] RefreshTokenCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _refreshTokenHandler.HandleAsync(command, cancellationToken);
                return Ok(result);
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
                return Unauthorized(new { error = ex.Message });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutCommand command, CancellationToken cancellationToken)
        {
            try
            {
                await _logoutHandler.HandleAsync(command, cancellationToken);
                return Ok(new { message = "Refresh token successfully revoked." });
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

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailCommand command, CancellationToken cancellationToken)
        {
            try
            {
                await _confirmEmailHandler.HandleAsync(command, cancellationToken);
                return Ok(new { message = "Email confirmed successfully." });
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

        [HttpPost("resend-confirmation")]
        public async Task<IActionResult> ResendConfirmation([FromBody] ResendConfirmationEmailCommand command, CancellationToken cancellationToken)
        {
            try
            {
                await _resendConfirmationEmailHandler.HandleAsync(command, cancellationToken);
                return Ok(new { message = "Confirmation email resent successfully." });
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
        
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command, CancellationToken cancellationToken)
        {
            try
            {
                await _forgotPasswordHandler.HandleAsync(command, cancellationToken);
                return Ok(new { message = "Password reset link has been sent." });
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
        
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command, CancellationToken cancellationToken)
        {
            try
            {
                await _resetPasswordHandler.HandleAsync(command, cancellationToken);
                return Ok(new { message = "Password has been reset successfully." });
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
        
        [HttpPost("2fa/setup")]
        [Authorize]
        public async Task<IActionResult> SetupTwoFactor([FromServices] GenerateTwoFactorQrCodeCommandHandler handler, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId(); 
            var response = await _generateTwoFactorQrCodeHandler.HandleAsync(new GenerateTwoFactorQrCodeCommand { UserId = userId });
            return Ok(response);
        }
        
        [Authorize]
        [HttpPost("manage/2fa/enable")]
        public async Task<IActionResult> Enable2Fa([FromBody] Enable2FaCommand command)
        {
            try
            {
                command.UserId = User.GetUserId().ToString(); 
                await _enable2FaHandler.HandleAsync(command);
                return Ok(new { message = "Two-factor authentication enabled." });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        
        [Authorize]
        [HttpPost("manage/2fa/disable")]
        public async Task<IActionResult> Disable2Fa([FromBody] Disable2FaCommand command)
        {
            try
            {
                var userId = User.GetUserId();
                await _disable2FaHandler.HandleAsync(userId, command);
                return Ok(new { message = "Two-factor authentication disabled." });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        
        [Authorize]
        [HttpPost("manage/2fa/recovery-codes")]
        public async Task<IActionResult> RegenerateRecoveryCodes()
        {
            try
            {
                var userId = User.GetUserId();
                var codes = await _regenerateRecoveryCodesHandler.HandleAsync(userId);
                return Ok(codes);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("manage/2fa/recovery-codes")]
        public async Task<IActionResult> GetRecoveryCodes()
        {
            try
            {
                var userId = User.GetUserId();
                var codes = await _getRecoveryCodesHandler.HandleAsync(userId);
                return Ok(codes);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
