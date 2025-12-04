namespace Laboratory_Service.Application.DTOs.TestResult
{
    /// <summary>
    /// DTO for response after processing test result message.
    /// </summary>
    public class TestResultIngressResponseDto
    {
        /// <summary>
        /// Indicates whether the message was processed successfully.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Response message (success or error description).
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Number of test results created.
        /// </summary>
        public int CreatedCount { get; set; }

        /// <summary>
        /// Message ID that was processed (for reference).
        /// </summary>
        public string MessageId { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when the message was processed.
        /// </summary>
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Optional error details if processing failed.
        /// </summary>
        public string? ErrorDetails { get; set; }
    }
}

