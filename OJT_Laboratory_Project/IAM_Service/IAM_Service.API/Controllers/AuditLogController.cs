using IAM_Service.Application.AuditLogs.Querry;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IAM_Service.API.Controllers
{
    /// <summary>
    /// create api to view all log
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLogController : ControllerBase
    {
        /// <summary>
        /// The mediator
        /// </summary>
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditLogController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator.</param>
        public AuditLogController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Gets all audit log.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllAuditLog()
        {
            var query = new GetAuditLogQuery();
            return Ok(await _mediator.Send(query));
        }
    }
}
