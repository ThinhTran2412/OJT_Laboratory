namespace Simulator.Domain.Entity
{
    /// <summary>
    /// Create Raw Test Result Entity
    /// </summary>
    public class RawTestResult
    {
        /// <summary>
        /// Unique identifier for the test result.
        /// </summary>
        /// <value>
        /// The test result identifier.
        /// </value>
        public int TestResultId { get; set; }

        /// <summary>
        /// Foreign key to the associated test order.
        /// </summary>
        /// <value>
        /// The test order identifier.
        /// </value>
        public Guid TestOrderId { get; set; }

        /// <summary>
        /// Standardized test code (e.g., LOINC code) if available.
        /// </summary>
        /// <value>
        /// The test code.
        /// </value>
        public string TestCode { get; set; } = string.Empty;

        /// <summary>
        /// Parameter/Analyte name within the test (e.g., Hemoglobin for CBC panel).
        /// </summary>
        /// <value>
        /// The parameter.
        /// </value>
        public string Parameter { get; set; } = string.Empty;

        /// <summary>
        /// Numeric value of the result (if applicable).
        /// </summary>
        /// <value>
        /// The value numeric.
        /// </value>
        public double? ValueNumeric { get; set; }

        /// <summary>
        /// Text value of the result (for non-numeric or qualitative results).
        /// </summary>
        /// <value>
        /// The value text.
        /// </value>
        public string? ValueText { get; set; }

        /// <summary>
        /// Unit of measurement for the result (e.g., g/dL, mmol/L).
        /// </summary>
        /// <value>
        /// The unit.
        /// </value>
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// Reference range for the result (display string).
        /// </summary>
        /// <value>
        /// The reference range.
        /// </value>
        public string ReferenceRange { get; set; } = string.Empty;

        /// <summary>
        /// Instrument name used to perform the test.
        /// </summary>
        /// <value>
        /// The instrument.
        /// </value>
        public string Instrument { get; set; } = string.Empty;

        /// <summary>
        /// Datetime when the measurement was performed.
        /// </summary>
        /// <value>
        /// The performed date.
        /// </value>
        public DateTime PerformedDate { get; set; }

        /// <summary>
        /// Status of the result: Completed | Reviewed.
        /// </summary>
        /// <value>
        /// The result status.
        /// </value>
        public string ResultStatus { get; set; } = string.Empty;

        /// <summary>
        /// UserId of performer.
        /// </summary>
        /// <value>
        /// The performed by user identifier.
        /// </value>
        public int? PerformedByUserId { get; set; }

        /// <summary>
        /// UserId of reviewer (if reviewed).
        /// </summary>
        /// <value>
        /// The reviewed by user identifier.
        /// </value>
        public int? ReviewedByUserId { get; set; }

        /// <summary>
        /// Review datetime (if reviewed).
        /// </summary>
        /// <value>
        /// The reviewed date.
        /// </value>
        public DateTime? ReviewedDate { get; set; }
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string Status { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is sent.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is sent; otherwise, <c>false</c>.
        /// </value>
        public bool IsSent { get; set; } = false;
        /// <summary>
        /// Gets or sets the created at.
        /// </summary>
        /// <value>
        /// The created at.
        /// </value>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
