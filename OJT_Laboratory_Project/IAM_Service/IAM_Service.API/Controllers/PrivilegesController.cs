using IAM_Service.Application.Privileges.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace IAM_Service.API.Controllers
{
    /// <summary>
    /// API controller for managing privileges.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PrivilegesController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrivilegesController"/> class.
        /// </summary>
        /// <param name="mediator">The MediatR mediator for handling queries.</param>
        public PrivilegesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves a list of all available privileges.
        /// This endpoint is typically used to populate dropdown filters for role management.
        /// </summary>
        /// <returns>A list of all privileges in the system.</returns>
        [HttpGet]
        [SwaggerOperation(
            Summary = "Get list of privileges",
            Description = "Retrieves a list of all available privileges for use in filters and dropdowns."
        )]
        public async Task<IActionResult> Get()
        {
            var query = new GetPrivilegesQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
