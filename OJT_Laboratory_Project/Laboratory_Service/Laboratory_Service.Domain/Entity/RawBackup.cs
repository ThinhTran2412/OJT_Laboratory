namespace Laboratory_Service.Domain.Entity
{
    /// <summary>
    /// Create entity RawBackup
    /// </summary>
    public class RawBackup
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the content of the raw.
        /// </summary>
        /// <value>
        /// The content of the raw.
        /// </value>
        public string RawContent { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the received at.
        /// </summary>
        /// <value>
        /// The received at.
        /// </value>
        public DateTime ReceivedAt { get; set; }
    }
}
