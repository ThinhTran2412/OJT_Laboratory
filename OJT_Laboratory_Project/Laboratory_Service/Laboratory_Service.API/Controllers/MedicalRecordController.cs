using Laboratory_Service.Application.MedicalRecords.Commands;
using Laboratory_Service.Application.MedicalRecords.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Laboratory_Service.API.Controllers
{
    /// <summary>
    /// Controller for Medical Record management operations
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("api/[controller]")]
    [ApiController]
    // JWT authentication disabled
    // [Authorize]
    public class MedicalRecordController : ControllerBase
    {
        /// <summary>
        /// The mediator
        /// </summary>
        private readonly IMediator _mediator;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<MedicalRecordController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MedicalRecordController" /> class.
        /// </summary>
        /// <param name="mediator">The mediator.</param>
        /// <param name="logger">The logger.</param>
        public MedicalRecordController(IMediator mediator, ILogger<MedicalRecordController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Adds the medical record.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddMedicalRecord([FromBody] AddMedicalRecordCommand request)
        {
            var userNameClaim = User?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            if (!string.IsNullOrEmpty(userNameClaim))
            {
                request.CreatedBy = userNameClaim;
            }

            var medicalReocrdId = await _mediator.Send(request);
            return Ok(medicalReocrdId);
        }
        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            //var userRoles = User?.FindAll(System.Security.Claims.ClaimTypes.Role).Select(r => r.Value).ToList();

            //if (userRoles == null || !userRoles.Contains("1"))
            //    return Forbid("You are not authorized to access this resource.");

            var records = await _mediator.Send(new GetAllMedicalRecordsQuery());
            return Ok(records);
        }

        /// <summary>
        /// Gets a medical record by id.
        /// </summary>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userRoles = User?.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
            if (userRoles == null || !userRoles.Contains("1"))
                return Forbid("You are not authorized to access this resource.");

            try
            {
                var record = await _mediator.Send(new GetMedicalRecordByIdQuery(id));
                return Ok(record);
            }
            catch (KeyNotFoundException knf)
            {
                _logger.LogWarning(knf, "Medical record not found: {Id}", id);
                return NotFound(new { message = knf.Message });
            }
        }
        /// <summary>
        /// Updates an existing medical record.
        /// </summary>
        /// <remarks>
        /// Only authorized roles (e.g. admin, doctor, lab) are allowed to update records.
        /// The <paramref name="id"/> path value will be assigned to the <see cref="UpdateMedicalRecordCommand.MedicalRecordId"/>.
        /// The handler is expected to perform validation and return the updated record or throw appropriate exceptions.
        /// </remarks>
        /// <param name="id">Medical record identifier.</param>
        /// <param name="request">Update command containing fields to modify.</param>
        /// <returns>Updated medical record DTO on success; proper error response on failure.</returns>
        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateMedicalRecord(int id, [FromBody] UpdateMedicalRecordCommand request)
        {
            request.MedicalRecordId = id;
            var userNameClaim = User?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            if (!string.IsNullOrEmpty(userNameClaim))
            {
                request.UpdatedBy = userNameClaim;
            }
            var result = await _mediator.Send(request);
            return Ok(result);
        }


        /// <summary>
        /// Deletes (soft-delete) a medical record by id.
        /// </summary>
        /// <remarks>
        /// Only users with administrative privileges (example role "1") are permitted to delete records.
        /// The operation performs safety checks (e.g. pending orders) and creates an audit entry when deletion succeeds.
        /// </remarks>
        /// <param name="id">Identifier of the medical record to delete.</param>
        /// <returns>Result object with success flag and optional message.</returns>
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicalRecord(int id)
        {
            var userRoles = User?.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
            if (userRoles == null || !userRoles.Contains("1"))
                return Forbid("You are not authorized to delete medical records.");
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User?.FindFirst(ClaimTypes.Name)?.Value;
            var identifyNumber = User?.FindFirst("identifyNumber")?.Value;

            var deletedBy = !string.IsNullOrEmpty(userName) ? userName :
                            !string.IsNullOrEmpty(userId) ? userId :
                            !string.IsNullOrEmpty(identifyNumber) ? identifyNumber : "unknown";

            var command = new DeleteMedicalRecordCommand
            {
                MedicalRecordId = id,
                DeletedBy = deletedBy
            };

            try
            {
                var result = await _mediator.Send(command);
                if (!result.Success)
                    return BadRequest(new { message = result.Message });

                return Ok(result);
            }
            catch (KeyNotFoundException knf)
            {
                return NotFound(new { message = knf.Message });
            }
            catch (InvalidOperationException inv)
            {
                return Conflict(new { message = inv.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting medical record {Id}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the medical record." });
            }

        }
    }
}
