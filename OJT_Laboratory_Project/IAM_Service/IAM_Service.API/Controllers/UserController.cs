using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using IAM_Service.Application.Users.Command;
using IAM_Service.Application.Users.Queries;
using IAM_Service.Application.Users.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

/// <summary>
/// Controller managing user-related operations, such as user creation.
/// </summary>
namespace IAM_Service.API.Controllers
{
    /// <summary>
    /// Create api for create user
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// The mediator
        /// </summary>
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController" /> class.
        /// </summary>
        /// <param name="mediator">The mediator.</param>
        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates the user.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        [HttpPost("create")]
        [Authorize(Policy = "CanCreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Send the create user command to the mediator
            await _mediator.Send(command);

            return CreatedAtAction(nameof(CreateUser), new { email = command.Email }, new
            {
                message = "User created successfully, password has been sent to email.",
                email = command.Email
            });
        }

        /// <summary>
        /// Get user detail by email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        [HttpGet("detail")]
        public async Task<IActionResult> GetUserDetail([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest(new { message = "Email is required." });

            var emailFromClaims =
                User.FindFirst(ClaimTypes.Email)?.Value
                ?? User.FindFirst(JwtRegisteredClaimNames.Email)?.Value;

            var isSelf = !string.IsNullOrWhiteSpace(emailFromClaims) &&
                         string.Equals(emailFromClaims, email, StringComparison.OrdinalIgnoreCase);

            
            var hasViewPrivilege = User.HasClaim("privilege", "VIEW_USER");
            if (!isSelf && !hasViewPrivilege)
                return Forbid();

            var result = await _mediator.Send(new GetUserDetailQuery(email));
            if (result == null)
                return NotFound(new { message = "The account does not exist or has been deleted." });

            return Ok(result);
        }


        /// <summary>
        /// Gets the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        [HttpGet("getListOfUser")]
        [SwaggerOperation(Summary = "Get list of Users", Description = "Get list by query params")]
        [Authorize(Policy = "CanViewUser")]
        public async Task<IActionResult> Get([FromQuery] GetUserQuery query)
        {
            var result = await _mediator.Send(query);

            if (result == null || result.Count == 0)
                return Ok(new { Message = "No Data", Data = new List<object>() });

            return Ok(result);
        }

        [HttpGet("detailByIdentify")]
        public async Task<IActionResult> GetUserDetailByIdentify([FromQuery] string identifyNumber)
        {
            if (string.IsNullOrWhiteSpace(identifyNumber))
                return BadRequest(new { message = "Identify Number is required." });

            var hasRequiredScope = User.HasClaim("scope", "iam_user_read");
            if (!hasRequiredScope)
            {
                return StatusCode(403, new
                {
                    error = "insufficient_scope",
                    message = "Service token lacks the required 'iam_user_read' scope."
                });
            }

            var result = await _mediator.Send(new GetUserDetailByIdentifyQuery(identifyNumber));

            if (result == null)
                return NotFound(new { message = $"The user with Identify Number {identifyNumber} does not exist." });

            return Ok(result);
        }
        /// <summary>
        /// Update an existing user�s information.
        /// </summary>
        /// <param name="command">The update user command.</param>
        /// <returns></returns>
        [HttpPatch("update")]
        [Authorize(Policy = "CanModifyUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserCommand command)
        {
            try
            {
                await _mediator.Send(command);
                return Ok(new { message = "User information updated successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Unexpected errors
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deletes the user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        [HttpDelete("delete/{userId:int}")]
        [Authorize(Policy = "CanDeleteUser")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                await _mediator.Send(new DeleteUserCommand { UserId = userId });
                return Ok(new { message = "User deleted successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        /// <summary>
        /// Gets the user profile.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        [HttpGet("getUserProfile")]
        [Authorize]
        public async Task<IActionResult> GetUserProfile([FromQuery] int userId)
        {
            if (userId <= 0)
                return BadRequest(new { message = "UserId is required." });

            var result = await _mediator.Send(new GetUserProfileQuery(userId));
            return Ok(result);
        }
        [HttpPatch("updateUserProfile")]
        [Authorize]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateUserProfileCommand command)
        {
            try
            {
                await _mediator.Send(command);
                return Ok(new { message = "User profile updated successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Unexpected errors
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
