using IAM_Service.Application.Registers.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IAM_Service.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RegistersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Register a new user account (Public registration for patients)
        /// </summary>
        /// <param name="command">Registration request containing FullName, Email, IdentifyNumber, Password</param>
        /// <returns>201 Created if successful</returns>
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegistersAccountCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _mediator.Send(command);
            return StatusCode(201, new
              {
                 message = "Registration successful. Please log in to continue."
              });
        }
    }
}
