namespace Laboratory_Service.Domain.Entity
{
    /// <summary>
    /// Entity to track processed messages for idempotency.
    /// Prevents duplicate processing of the same message.
    /// </summary>
    public class ProcessedMessage
    {
        /// <summary>
        /// Unique identifier for the processed message record.
        /// </summary>
        public int ProcessedMessageId { get; set; }

        /// <summary>
        /// Unique message identifier from the external system.
        /// Used as the key for idempotency checking.
        /// </summary>
        public string MessageId { get; set; } = string.Empty;

        /// <summary>
        /// Source system identifier (e.g., "LabInstrument1", "ExternalLab").
        /// </summary>
        public string SourceSystem { get; set; } = string.Empty;

        /// <summary>
        /// Test Order ID associated with the processed message.
        /// </summary>
        public Guid TestOrderId { get; set; }

        /// <summary>
        /// Timestamp when the message was processed.
        /// </summary>
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Number of test results created from this message.
        /// </summary>
        public int CreatedCount { get; set; }
    }
}

