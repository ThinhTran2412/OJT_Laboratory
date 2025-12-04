namespace Simulator.Application.DTOs
{
    /// <summary>
    /// Create Raw Test Result DTO
    /// </summary>
    public class RawTestResultDTO
    {
        /// <summary>
        /// Gets or sets the test order identifier.
        /// </summary>
        /// <value>
        /// The test order identifier.
        /// </value>
        public Guid TestOrderId { get; set; }
        /// <summary>
        /// Gets or sets the instrument.
        /// </summary>
        /// <value>
        /// The instrument.
        /// </value>
        public string Instrument { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the performed date.
        /// </summary>
        /// <value>
        /// The performed date.
        /// </value>
        public DateTime PerformedDate { get; set; }
        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        /// <value>
        /// The results.
        /// </value>
        public List<RawResultItemDTO> Results { get; set; } = new();
    }
}
