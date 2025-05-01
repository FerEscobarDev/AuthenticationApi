using AuthenticationApi.Application.Commands.RegisterUser;
using AuthenticationApi.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationApi.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly RegisterUserCommandHandler _registerHandler;

        public AuthController(RegisterUserCommandHandler registerHandler)
        {
            _registerHandler = registerHandler;
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
    }
}
