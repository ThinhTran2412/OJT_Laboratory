using Laboratory_Service.Application.Comments.Commands;
using Laboratory_Service.Application.Comments.Queries;
using Laboratory_Service.Application.DTOs.Comment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace Laboratory_Service.API.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        /// <summary>
        /// The mediator
        /// </summary>
        private readonly IMediator _mediator;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<CommentController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator.</param>
        /// <param name="logger">The logger.</param>
        public CommentController(IMediator mediator, ILogger<CommentController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Add new comment for TestOrder or TestResult
        /// </summary>
        /// <param name="dto">The dto.</param>
        /// <returns></returns>
        [HttpPost("add")]
        [Authorize(Policy = "CanAddComment")]
        public async Task<IActionResult> Add([FromBody] CreateCommentDto dto)
        {
            _logger.LogInformation("Starting Add Comment request...");

            // Lấy UserId từ JWT
            var userName = User?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            if(userName == null)
            {
                return Unauthorized(new { Message = "Invalid token or userName not found in token." });
            }

            var command = new AddCommentCommand(dto, userName);
            var result = await _mediator.Send(command);

            _logger.LogInformation("Comment created successfully with ID = {CommentId}", result);

            return Ok(new
            {
                CommentId = result,
                Message = "Comment added successfully"
            });
        }


        /// <summary>
        /// Soft delete a comment
        /// </summary>
        /// <param name="commentId">The comment identifier.</param>
        /// <returns></returns>
        [HttpDelete("delete/{commentId}")]
        [Authorize]
        public async Task<IActionResult> Delete(int commentId)
        {
            // Lấy userId từ JWT (thử "userId" trước, fallback sang ClaimTypes.NameIdentifier)
            var userName = User?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            if (userName == null)
            {
                return Unauthorized(new { Message = "Invalid token or userName not found in token." });
            }


            _logger.LogInformation("Delete Comment request for ID = {CommentId} by user {DeletedBy}",
                commentId, userName);

            // DTO chỉ chứa CommentId (DeletedBy không còn trong DTO)
            var dto = new DeleteCommentDto
            {
                CommentId = commentId
            };

            // Tạo command với dto và deletedBy (lấy từ JWT)
            var command = new DeleteCommentCommand(dto, userName);
            var success = await _mediator.Send(command);

            if (!success)
            {
                _logger.LogWarning("Delete failed: Comment {CommentId} not found or already deleted", commentId);

                return NotFound(new
                {
                    Message = "Comment not found or already deleted"
                });
            }

            _logger.LogInformation("Comment {CommentId} deleted successfully by {DeletedBy}",
                commentId, userName);

            return Ok(new
            {
                Message = "Comment deleted successfully"
            });
        }
        [HttpPut("modify")]
        [Authorize(Policy = "CanModifyComment")]
        public async Task<IActionResult> ModifyComment(int commentId, [FromBody] ModifyCommentCommand request)
        {
            if (commentId <= 0)
                return BadRequest("commentId must be greater than 0.");

            request.CommentId = commentId;

            var userName = User?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            if (userName == null)
            {
                return Unauthorized(new { Message = "Invalid token or userName not found in token." });
            }

            request.ModifiedBy = userName;

            var result = await _mediator.Send(request);
            return Ok(result);
        }

        /// <summary>
        /// Get comments by test result IDs (from request body)
        /// </summary>
        /// <param name="request">Request containing list of test result IDs</param>
        /// <returns></returns>
        [HttpPost("by-test-results")]
        public async Task<IActionResult> GetCommentsByTestResults([FromBody] GetCommentsByTestResultsRequest request)
        {
            if(request == null || request.TestResultIds == null || !request.TestResultIds.Any())
            {
               return BadRequest("testResultIds cannot be null or empty.");
            }

            var result = await _mediator.Send(new GetCommentsQuery(request.TestResultIds));
            return Ok(result);
        }

        /// <summary>
        /// Get comments by test order ID
        /// </summary>
        /// <param name="testOrderId">Test order ID</param>
        /// <returns></returns>
        [HttpGet("by-test-order/{testOrderId}")]
        public async Task<IActionResult> GetCommentsByTestOrderId(Guid testOrderId)
        {
            if(testOrderId == Guid.Empty)
            {
                return BadRequest("testOrderId cannot be empty.");
            }

            var result = await _mediator.Send(new GetCommentsByTestOrderIdQuery(testOrderId));
            return Ok(result);
        }

    }

    /// <summary>
    /// Request model for getting comments by test result IDs
    /// </summary>
    public class GetCommentsByTestResultsRequest
    {
        public List<int> TestResultIds { get; set; } = new List<int>();
    }
}
