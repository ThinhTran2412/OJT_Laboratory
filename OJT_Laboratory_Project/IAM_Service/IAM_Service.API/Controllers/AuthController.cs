using IAM_Service.Application.ClientCredentials;
using IAM_Service.Application.forgot_password.Command;
using IAM_Service.Application.Login;
using IAM_Service.Application.Logout;
using IAM_Service.Application.RefreshTokens.Command;
using IAM_Service.Application.ResetPassword;
using MediatR;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller processing authentication-related requests, such as user login.
/// </summary>
namespace IAM_Service.API.Controllers
{
    /// <summary>
    /// Handles authentication-related requests such as user login.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        /// <summary>
        /// The mediator
        /// </summary>
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController" /> class.
        /// </summary>
        /// <param name="mediator">The mediator.</param>
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Logins the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            var token = await _mediator.Send(command);
            return Ok(token);
        }
        /// <summary>
        /// Refreshes the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand request)
        {
            var result = await _mediator.Send(new RefreshTokenCommand
            {
                RefreshToken = request.RefreshToken
            });

            return Ok(result);
        }


        /// <summary>
        /// Logouts the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutCommand command)
        {
            await _mediator.Send(command);
            return Ok(new { message = "Logged out successfully" });
        }
        [HttpPost("connect/token")]
        [Consumes("application/x-www-form-urlencoded")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        public async Task<IActionResult> ClientCredentials([FromForm] ClientCredentialsCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = "invalid_client", error_description = ex.Message });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "server_error" });
            }
        }
        /// <summary>
        /// Forgots the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        [HttpPost("forgot_password")]
        public async Task<IActionResult> Forgot([FromBody] ForgotCommand command)
        {
            await _mediator.Send(command);
            return Ok(new { message = "email have been seen!" });
        }

        [HttpPost("reset_password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            await _mediator.Send(command);
            return Ok(new { message = "Password reset successfully" });
        }
    }
}
