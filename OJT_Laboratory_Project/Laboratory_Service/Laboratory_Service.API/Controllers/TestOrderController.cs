using Laboratory_Service.Application.TestOrders.Commands;
using Laboratory_Service.Application.TestOrders.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;


namespace Laboratory_Service.API.Controllers
{
    /// <summary>
    /// Create many endpoint for api CRUD TestOrder
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("api/[controller]")]
    [ApiController]
    public class TestOrderController : ControllerBase
    {
        /// <summary>
        /// The mediator
        /// </summary>
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestOrderController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator.</param>
        public TestOrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Create a new test order for a patient.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        [HttpPost]
        ///[Authorize(Policy = "CanCreateTestOrder")]
        [SwaggerOperation(Summary = "Create a new test order", Description = "Creates a test order; handles patient/medical record per business rules.")]
        public async Task<IActionResult> Create([FromBody] CreateTestOrderCommand command)
        {
            try
            {
                var userNameClaim = User?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
                if (!string.IsNullOrEmpty(userNameClaim))
                {
                    command.CreatedBy = userNameClaim;
                }
                else
                {
                    command.CreatedBy = "System";
                }

                var orderId = await _mediator.Send(command);
                return Ok(new { orderId = orderId });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception - will be caught by global exception handler
                return StatusCode(500, new { message = "An error occurred while creating the test order.", error = ex.Message });
            }
        }

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize(Policy = "CanViewTestOrders")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetTestOrderDetailQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound(new { message = "Test order not found or has been deleted." });
            }

            return Ok(result);
        }

        /// <summary>
        /// Modify an existing Test Order for a patient.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        [Authorize(Policy = "CanModifyTestOrder")]
        public async Task<IActionResult> ModifyTestOrder(string id, [FromBody] ModifyTestOrderCommand command)
        {
            if (!Guid.TryParse(id, out var parsedId))
            {
                return BadRequest(new { Message = $"Invalid TestOrderId: {id}" });
            }

            var userName = User?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

            if (command.TestOrderId != parsedId)
                return BadRequest("TestOrderId in URL and body do not match.");

            if (!string.IsNullOrEmpty(userName))
            {
                command.UpdatedBy = userName;
            }

            var result = await _mediator.Send(command);

            return Ok(new
            {
                message = "TestOrder modified successfully.",
                testOrderId = parsedId
            });
        }



        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "CanDeleteTestOrder")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!Guid.TryParse(id, out var parsedId))
            {
                return BadRequest(new { Message = $"Invalid TestOrderId: {id}" });
            }
            var userName = User?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

            var command = new DeleteTestOrderCommand(parsedId, userName!);
            var result = await _mediator.Send(command);

            return result
                ? Ok(new { Message = "Test order deleted successfully." })
                : BadRequest(new { Message = "Failed to delete test order." });
        }
        /// <summary>
        /// Get paged list of patient test orders with optional search, filtering, and sorting.
        /// Query params:
        /// - search: keyword across patient name, phone number, status, or user names
        /// - page, pageSize: paging
        /// - sortBy: id|patientName|age|gender|phoneNumber|status|createdDate|runDate; sortDesc: true|false
        /// - status: filter by status (Pending, Cancelled, Completed)
        /// Default sorting is by createdDate descending (most recent first).
        /// Returns "No Data" message when no records are found.
        /// </summary>
        [HttpGet("getList")]
        [SwaggerOperation(
            Summary = "Get list of patient test orders",
            Description = "Get paginated list of patient test orders with search, filter, and sort capabilities. Default sort is by createdDate descending."
        )]
        [Authorize(Policy = "CanViewTestOrders")]
        public async Task<IActionResult> Get([FromQuery] GetTestOrdersQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get detailed information of a specific patient test order by ID.
        /// Returns patient information (from MedicalRecord) and test order details.
        /// If the test order is not found, returns 404 Not Found.
        /// </summary>
        [HttpGet("detail/{id:guid}")]
        [SwaggerOperation(
            Summary = "Get patient test order detail",
            Description = "Get detailed information of a patient test order including patient info and test order details."
        )]
        [Authorize(Policy = "CanViewTestOrders")]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            var query = new GetTestOrderDetailQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound(new { message = "Test order not found or has been deleted." });
            }

            return Ok(result);
        }

        /// <summary>
        /// Get test results for a specific test order by test order ID.
        /// Returns a list of test results associated with the specified test order.
        /// </summary>
        [HttpGet("{testOrderId:guid}/test-results")]
        [SwaggerOperation(
            Summary = "Get test results by test order ID",
            Description = "Get all test results for a specific test order identified by test order ID."
        )]
        [Authorize(Policy = "CanViewTestOrders")]
        public async Task<IActionResult> GetTestResults(Guid testOrderId)
        {
            var query = new GetTestResultsByTestOrderIdQuery(testOrderId);
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        /// Get all test orders for a specific patient by patient ID.
        /// Returns a list of test orders associated with the specified patient.
        /// </summary>
        [HttpGet("by-patient/{patientId:int}")]
        [SwaggerOperation(
            Summary = "Get test orders by patient ID",
            Description = "Get all test orders for a specific patient identified by patient ID."
        )]
        [Authorize]
        public async Task<IActionResult> GetByPatientId(int patientId)
        {
            var query = new GetTestOrdersByPatientIdQuery(patientId);
            var result = await _mediator.Send(query);

            if (result == null || !result.Any())
            {
                return Ok(new { message = "No test orders found for this patient.", items = new List<object>() });
            }

            return Ok(result);
        }

        /// <summary>
        /// Export selected test orders or all orders from current month to Excel file.
        /// Used by regular users to export their own test orders.
        /// Business rules:
        /// - If testOrderIds is empty/null, export all test orders from current month
        /// - If testOrderIds has values, export only those specific orders
        /// - Run By and Run On columns are empty if Status is not "Completed"
        /// - File naming: "Test Orders-{PatientName}-{ExportDate}.xlsx" or custom name
        /// </summary>
        [HttpPost("export-patient/{patientId:int}")]
        [SwaggerOperation(
            Summary = "Export selected patient test orders to Excel",
            Description = "Export specific test orders or all orders from current month for a patient to Excel file. If no test order IDs provided, exports all orders from current month (per policy).",
            Tags = new[] { "Export" }
        )]
        [Authorize]
        public async Task<IActionResult> ExportSelectedTestOrders(
            int patientId,
            [FromBody] ExportTestOrdersRequest request)
        {
            try
            {
                var query = new ExportSelectedTestOrdersQuery(patientId)
                {
                    TestOrderIds = request.TestOrderIds,
                    FileName = request.FileName
                };

                var excelBytes = await _mediator.Send(query);

                // Generate filename
                var finalFileName = !string.IsNullOrWhiteSpace(request.FileName)
                    ? SanitizeFileName(request.FileName)
                    : request.TestOrderIds != null && request.TestOrderIds.Any()
                        ? $"Test Orders-Patient{patientId}-{request.TestOrderIds.Count}orders-{DateTime.UtcNow:yyyy-MM-dd}.xlsx"
                        : $"Test Orders-Patient{patientId}-{DateTime.UtcNow:yyyy-MM}.xlsx";

                // Ensure .xlsx extension
                if (!finalFileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    finalFileName += ".xlsx";
                }

                return File(excelBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    finalFileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Export failed: {ex.Message}" });
            }
        }

        /// <summary>
        /// Sanitizes the filename to remove invalid characters.
        /// </summary>
        private static string SanitizeFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            return string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary>
        /// Export test orders for a specific patient to Excel file.
        /// Used by regular users to export their own test orders.
        /// Business rules:
        /// - Run By and Run On columns are empty if Status is not "Completed"
        /// - File naming: "Test Orders-{PatientName}-{ExportDate}.xlsx" or custom name
        /// </summary>
        [HttpGet("export-patient/{patientId:int}")]
        [SwaggerOperation(
            Summary = "Export patient test orders to Excel",
            Description = "Export all test orders for a specific patient to Excel file. Returns Excel file as download.",
            Tags = new[] { "Export" }
        )]
        [Authorize] // Regular users can export their own test orders
        public async Task<IActionResult> ExportByPatientId(int patientId, [FromQuery] string? fileName = null)
        {
            var query = new ExportTestOrdersByPatientIdQuery(patientId)
            {
                FileName = fileName
            };

            var excelBytes = await _mediator.Send(query);

            // Generate filename: "Test Orders-{PatientName}-{ExportDate}.xlsx" or use custom name
            var finalFileName = !string.IsNullOrWhiteSpace(fileName)
                ? SanitizeFileName(fileName)
                : $"Test Orders-Patient{patientId}-{DateTime.UtcNow:yyyy-MM-dd}.xlsx";

            // Ensure .xlsx extension
            if (!finalFileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                finalFileName += ".xlsx";
            }

            return File(excelBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                finalFileName);
        }

    }
}
