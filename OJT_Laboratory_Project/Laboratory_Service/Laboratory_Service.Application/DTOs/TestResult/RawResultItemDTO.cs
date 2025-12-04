namespace Laboratory_Service.Application.DTOs.TestResult
{
    /// <summary>
    /// Creates the raw result item dto.
    /// </summary>
    public class RawResultItemDTO
    {
        /// <summary>
        /// Gets or sets the test code.
        /// </summary>
        /// <value>
        /// The test code.
        /// </value>
        public string TestCode { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the parameter.
        /// </summary>
        /// <value>
        /// The parameter.
        /// </value>
        public string Parameter { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the value numeric.
        /// </summary>
        /// <value>
        /// The value numeric.
        /// </value>
        public double? ValueNumeric { get; set; }
        /// <summary>
        /// Gets or sets the value text.
        /// </summary>
        /// <value>
        /// The value text.
        /// </value>
        public string? ValueText { get; set; }
        /// <summary>
        /// Gets or sets the unit.
        /// </summary>
        /// <value>
        /// The unit.
        /// </value>
        public string Unit { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the reference range.
        /// </summary>
        /// <value>
        /// The reference range.
        /// </value>
        public string ReferenceRange { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string Status { get; set; } = "Completed";
    }
}
