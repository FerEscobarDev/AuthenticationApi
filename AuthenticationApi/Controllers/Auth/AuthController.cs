using AuthenticationApi.Application.Commands.LoginUser;
using AuthenticationApi.Application.Commands.RefreshToken;
using AuthenticationApi.Application.Commands.RegisterUser;
using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.DTOs.Auth;
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

        public AuthController(
            RegisterUserCommandHandler registerHandler, 
            LoginUserCommandHandler loginHandler,
            RefreshTokenCommandHandler refreshTokenHandler)
        {
            _registerHandler = registerHandler;
            _loginHandler = loginHandler;
            _refreshTokenHandler = refreshTokenHandler;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterUserCommand command)
        {
            try
            {
                var result = await _registerHandler.HandleAsync(command);
                return CreatedAtAction(nameof(Register), new { id = result.Id }, result);
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
    }
}
