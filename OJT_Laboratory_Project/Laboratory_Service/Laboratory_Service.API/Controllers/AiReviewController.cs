using Laboratory_Service.Application.AiReviewForTestOrder.Command;
using Laboratory_Service.Application.AiReviewForTestOrder.Querry;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Laboratory_Service.API.Controllers
{
    /// <summary>
    /// Ai Review
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("api/ai-review")]
    [ApiController]
    public class AiReviewController : ControllerBase
    {
        /// <summary>
        /// The mediator
        /// </summary>
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="AiReviewController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator.</param>
        public AiReviewController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Sets the ai review.
        /// </summary>
        /// <param name="testOrderId">The test order identifier.</param>
        /// <param name="enable">if set to <c>true</c> [enable].</param>
        /// <returns></returns>
        [HttpPut("{testOrderId}")]
        public async Task<IActionResult> SetAiReview(Guid testOrderId, [FromQuery] bool enable)
        {
            await _mediator.Send(new SetAiReviewModeForTestOrderCommand(testOrderId, enable));
            return Ok(new { TestOrderId = testOrderId, AiReviewEnabled = enable });
        }

        /// <summary>
        /// Gets the ai review status.
        /// </summary>
        /// <param name="testOrderId">The test order identifier.</param>
        /// <returns></returns>
        [HttpGet("{testOrderId}")]
        public async Task<IActionResult> GetAiReviewStatus(Guid testOrderId)
        {
            try
            {
                bool isEnabled = await _mediator.Send(new GetAiReviewModeForTestOrderQuery(testOrderId));
                return Ok(new { TestOrderId = testOrderId, AiReviewEnabled = isEnabled });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                {
                    return NotFound(new { Message = "Test order not found.", TestOrderId = testOrderId });
                }
                return BadRequest(new { Message = "Error getting AI review status.", Error = ex.Message });
            }
        }

        /// <summary>
        /// Triggers the ai review.
        /// </summary>
        /// <param name="testOrderId">The test order identifier.</param>
        /// <returns></returns>
        [HttpPost("{testOrderId}/trigger")]
        public async Task<IActionResult> TriggerAiReview(Guid testOrderId)
        {
            try
            {
                var reviewedOrder = await _mediator.Send(new TriggerAiReviewCommand(testOrderId));

                if (reviewedOrder == null)
                    return NotFound("Test order not found.");

                // Check if AI review was actually performed
                // If IsAiReviewEnabled is false, reviewedOrder will be returned but Status won't be "Reviewed By AI"
                if (reviewedOrder.Status != "Reviewed By AI" && reviewedOrder.IsAiReviewEnabled == false)
                {
                    return BadRequest(new
                    {
                        Message = "AI Review is not enabled for this test order. Please enable it first.",
                        TestOrderId = reviewedOrder.TestOrderId,
                        IsAiReviewEnabled = reviewedOrder.IsAiReviewEnabled
                    });
                }

                // Filter to only AI-reviewed results and distinct by TestResultId to avoid duplicates
                var results = reviewedOrder.TestResults
                    .Where(r => r.ReviewedByAI)
                    .GroupBy(r => r.TestResultId)
                    .Select(g => g.First())
                    .Select(r => new
                    {
                        r.TestResultId,
                        r.Parameter,
                        r.ValueNumeric,
                        r.ValueText,
                        r.Unit,
                        r.ReferenceRange,
                        r.ResultStatus,
                        r.ReviewedByAI,
                        r.AiReviewedDate
                    })
                    .ToList();

                return Ok(new
                {
                    TestOrderId = reviewedOrder.TestOrderId,
                    Status = reviewedOrder.Status,
                    IsAiReviewEnabled = reviewedOrder.IsAiReviewEnabled,
                    AiSummary = reviewedOrder.TempData["ai_summary"],
                    PredictedStatus = reviewedOrder.TempData["predicted_status"],
                    AiReviewedResults = results
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = "Error during AI review process.",
                    Error = ex.Message,
                    InnerException = ex.InnerException?.Message
                });
            }
        }

        /// <summary>
        /// Confirms the ai review results.
        /// </summary>
        /// <param name="testOrderId">The test order identifier.</param>
        /// <param name="confirmedByUserId">The confirmed by user identifier.</param>
        /// <returns></returns>
        [HttpPost("{testOrderId}/confirm")]
        public async Task<IActionResult> ConfirmAiReviewResults(Guid testOrderId, [FromQuery] int confirmedByUserId)
        {
            try
            {
                var confirmedOrder = await _mediator.Send(new ConfirmAiReviewResultsCommand(testOrderId, confirmedByUserId));

                if (confirmedOrder == null)
                    return NotFound("Test order not found.");

                var results = confirmedOrder.TestResults.Where(r => r.ReviewedByAI).Select(r => new
                {
                    r.TestResultId,
                    r.Parameter,
                    r.ValueNumeric,
                    r.ValueText,
                    r.Unit,
                    r.ReferenceRange,
                    r.ResultStatus,
                    r.ReviewedByAI,
                    r.AiReviewedDate,
                    r.IsConfirmed,
                    r.ConfirmedByUserId,
                    r.ConfirmedDate
                });

                return Ok(new
                {
                    TestOrderId = confirmedOrder.TestOrderId,
                    Status = confirmedOrder.Status,
                    ConfirmedResults = results
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = "Error during confirmation process.",
                    Error = ex.Message,
                    InnerException = ex.InnerException?.Message
                });
            }
        }
    }
}
