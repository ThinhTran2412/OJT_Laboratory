using Laboratory_Service.Application.Test_Result;
using Laboratory_Service.Application.Test_Result.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Laboratory_Service.API.Controllers
{
    /// <summary>
    /// Controller for TestResult management operations
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("api/[controller]")]
    [ApiController]
    public class TestResultController : ControllerBase
    {
        /// <summary>
        /// The mediator
        /// </summary>
        private readonly IMediator _mediator;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<TestResultController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestResultController" /> class.
        /// </summary>
        /// <param name="mediator">The mediator.</param>
        /// <param name="logger">The logger.</param>
        public TestResultController(IMediator mediator, ILogger<TestResultController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }



        /// <summary>
        /// Processes the test results from simulator.
        /// </summary>
        /// <param name="testOrderId">The test order identifier.</param>
        /// <returns></returns>
        [HttpPost("process-from-simulator")]
        [SwaggerOperation(
            Summary = "Process test results from Simulator",
            Description = "Processes test results from Simulator service via gRPC for the specified test order."
        )]
        public async Task<IActionResult> ProcessTestResultsFromSimulator(Guid testOrderId, string testType)
        {

            if (testOrderId == Guid.Empty)
            {

                return BadRequest(new { Message = "TestOrderId must be a valid, non-empty GUID." });
            }
            if (testType == null)
            {
                return BadRequest(new { Message = "TestType must be provided." });
            }

            var command = new ProcessTestResultMessageCommand { TestOrderId = testOrderId, TestType = testType };


            var response = await _mediator.Send(command);

            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            // [HttpPost("sync")]
            // public async Task<IActionResult> SyncRaw([FromBody] RawTestResultDTO dto)
            // {
            //     var result = await _mediator.Send(new SaveRawTestResultCommand(dto));
            //     return result ? Ok("Saved") : BadRequest("Error");

        }


        /// <summary>
        /// Export test results to PDF for printing.
        /// Business rules:
        /// - TestOrder must exist and not be deleted
        /// - TestOrder Status must be "Completed" or "Đã hoàn thành"
        /// - File naming: "Chi tiết-Tên bệnh nhân-Ngày in.pdf" or custom name
        /// - Creates PDF with two tables: Test Order Information and Test Results
        /// </summary>
        /// <param name="testOrderId">The test order identifier.</param>
        /// <param name="fileName">Optional custom filename.</param>
        /// <returns>
        /// PDF file as download.
        /// </returns>
        [HttpGet("print/{testOrderId:guid}")]
        [SwaggerOperation(
            Summary = "Print test results to PDF",
            Description = "Export test results to PDF for printing. Test order must exist, not be deleted, and have status 'Completed' or 'Đã hoàn thành'. Creates PDF with test order information and detailed test results."
        )]
        public async Task<IActionResult> PrintTestResults(Guid testOrderId, [FromQuery] string? fileName = null)
        {
            try
            {
                var query = new ExportTestResultsToPdfQuery(testOrderId)
                {
                    FileName = fileName
                };

                var pdfBytes = await _mediator.Send(query);

                // Get test order for filename generation
                var getOrderQuery = new Laboratory_Service.Application.TestOrders.Queries.GetTestOrderDetailQuery(testOrderId);
                var testOrder = await _mediator.Send(getOrderQuery);

                // Generate filename: "Chi tiết-Tên bệnh nhân-Ngày in.pdf" or use custom name
                var finalFileName = !string.IsNullOrWhiteSpace(fileName)
                    ? SanitizeFileName(fileName)
                    : $"Chi tiết-{(testOrder?.PatientName ?? "Patient")}-{DateTime.UtcNow:yyyy-MM-dd}.pdf";

                // Ensure .pdf extension
                if (!finalFileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    finalFileName += ".pdf";
                }

                return File(pdfBytes,
                    "application/pdf",
                    finalFileName);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation error when printing test results for test order {TestOrderId}", testOrderId);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error printing test results for test order {TestOrderId}", testOrderId);
                return StatusCode(500, new { message = "An error occurred while generating PDF." });
            }
        }

        /// <summary>
        /// Sanitizes the filename to remove invalid characters.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        private static string SanitizeFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            return string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
