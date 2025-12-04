using Laboratory_Service.Domain.Entity;

namespace Laboratory_Service.Application.Interface
{
    /// <summary>
    /// Repository interface for ProcessedMessage operations.
    /// Used for idempotency checking to prevent duplicate message processing.
    /// </summary>
    public interface IProcessedMessageRepository
    {
        /// <summary>
        /// Checks if a message with the given MessageId has already been processed.
        /// </summary>
        /// <param name="messageId">The message identifier to check.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The ProcessedMessage if found, null otherwise.</returns>
        Task<ProcessedMessage?> GetByMessageIdAsync(string messageId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new processed message record.
        /// This should be called after successfully processing a message.
        /// </summary>
        /// <param name="processedMessage">The processed message to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The added processed message.</returns>
        Task<ProcessedMessage> AddAsync(ProcessedMessage processedMessage, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a message exists and adds it atomically if it doesn't.
        /// This method uses database-level locking to prevent race conditions.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="sourceSystem">The source system.</param>
        /// <param name="testOrderId">The test order ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if the message was newly added, false if it already existed.</returns>
        Task<bool> TryAddIfNotExistsAsync(string messageId, string sourceSystem, Guid testOrderId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing processed message.
        /// </summary>
        /// <param name="processedMessage">The processed message to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated processed message.</returns>
        Task<ProcessedMessage> UpdateAsync(ProcessedMessage processedMessage, CancellationToken cancellationToken = default);
    }
}

