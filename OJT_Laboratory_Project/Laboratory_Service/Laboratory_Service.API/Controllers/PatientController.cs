using Laboratory_Service.Application.DTOs.Patients;
using Laboratory_Service.Application.Interface;
using Laboratory_Service.Application.Patients.Commands;
using Laboratory_Service.Application.Patients.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Laboratory_Service.API.Controllers
{
    /// <summary>
    /// Controller for Patient management operations
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("api/[controller]")]
    [ApiController]
    // JWT authentication disabled
    // [Authorize]
    public class PatientController : ControllerBase
    {
        /// <summary>
        /// The mediator
        /// </summary>
        private readonly IMediator _mediator;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<PatientController> _logger;
        /// <summary>
        /// The patient service
        /// </summary>
        private readonly IPatientService _patientService;


        /// <summary>
        /// Initializes a new instance of the <see cref="PatientController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="patientService">The patient service.</param>
        public PatientController(IMediator mediator, ILogger<PatientController> logger, IPatientService patientService)
        {
            _mediator = mediator;
            _logger = logger;
            _patientService = patientService;
        }

        /// <summary>
        /// Create a new patient
        /// </summary>
        /// <param name="dto">The dto.</param>
        /// <returns></returns>
        [HttpPost("create/by-identity")]
        public async Task<IActionResult> CreatePatientByIdentity([FromBody] CreatePatientClient dto) // Dùng DTO đầy đủ
        {
            try
            {
                var command = new CreatePatientByIdentityCommand
                {
                    IdentifyNumber = dto.IdentifyNumber,
                    FullName = dto.FullName,
                    DateOfBirth = dto.DateOfBirth,
                    Gender = dto.Gender,
                    PhoneNumber = dto.PhoneNumber,
                    Email = dto.Email,
                    Address = dto.Address
                };

                var result = await _mediator.Send(command);

                return CreatedAtAction(nameof(GetPatientById), new { id = result.PatientId }, result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Conflict or invalid operation");
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating patient by identity");
                return StatusCode(500, new { message = "An error occurred while creating the patient" });
            }
        }
        /// <summary>
        /// Gets the patient by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetPatientById(int id) => Ok();


        /// <summary>
        /// Gets my patient information.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyPatientInfo()
        {
            var identifyNumber = User.FindFirst("identifyNumber")?.Value;

            if (string.IsNullOrEmpty(identifyNumber))
                return BadRequest(new { message = "Token is missing IdentifyNumber claim." });

            var patient = await _patientService.GetPatientByIdentityNumberAsync(identifyNumber);

            if (patient == null)
                return NotFound(new { message = "Patient not found." });

            return Ok(patient);
        }

        /// <summary>
        /// Gets all patients.
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllPatients()
        {
            //var userId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            //var userName = User?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            //var userRoles = User?.FindAll(System.Security.Claims.ClaimTypes.Role).Select(r => r.Value).ToList();

            //if (userRoles == null || !userRoles.Contains("1"))
            //    return Forbid("You are not authorized to access this resource.");

            var patients = await _mediator.Send(new GetAllPatientsQuery());
            return Ok(patients);
        }


    }
}
