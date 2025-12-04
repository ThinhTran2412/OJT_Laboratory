namespace Laboratory_Service.Domain.Entity
{
    /// <summary>
    /// 
    /// </summary>
    public class TestResult
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
        /// Navigation to the parent test order.
        /// </summary>
        /// <value>
        /// The test order.
        /// </value>
        public TestOrder TestOrder { get; set; } = null!;

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
        /// Notes or reason for review.
        /// </summary>
        /// <value>
        /// The review notes.
        /// </value>
        public string? ReviewNotes { get; set; }

        /// <summary>
        /// Result flag label (e.g., Normal, Low, High, Critical).
        /// </summary>
        /// <value>
        /// The flag.
        /// </value>
        public string Flag { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when the flag was computed and applied (UTC).
        /// </summary>
        /// <value>
        /// The flagged at timestamp.
        /// </value>
        public DateTime? FlaggedAt { get; set; }

        /// <summary>
        /// UserId or system identifier who flagged the result.
        /// </summary>
        /// <value>
        /// The flagged by user identifier.
        /// </value>
        public int? FlaggedBy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [reviewed by ai].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [reviewed by ai]; otherwise, <c>false</c>.
        /// </value>
        public bool ReviewedByAI { get; set; } = false;

        /// <summary>
        /// Gets or sets the ai reviewed date.
        /// </summary>
        /// <value>
        /// The ai reviewed date.
        /// </value>
        public DateTime? AiReviewedDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is confirmed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is confirmed; otherwise, <c>false</c>.
        /// </value>
        public bool IsConfirmed { get; set; } = false;

        /// <summary>
        /// Gets or sets the confirmed by user identifier.
        /// </summary>
        /// <value>
        /// The confirmed by user identifier.
        /// </value>
        public int? ConfirmedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the confirmed date.
        /// </summary>
        /// <value>
        /// The confirmed date.
        /// </value>
        public DateTime? ConfirmedDate { get; set; }

        /// <summary>
        /// Date when the record was created.
        /// </summary>
        /// <value>
        /// The created at timestamp.
        /// </value>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// User who created the record.
        /// </summary>
        /// <value>
        /// The created by user identifier.
        /// </value>
        public int? CreatedBy { get; set; }

        /// <summary>
        /// Date when the record was last updated.
        /// </summary>
        /// <value>
        /// The updated at timestamp.
        /// </value>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// User who last updated the record.
        /// </summary>
        /// <value>
        /// The updated by user identifier.
        /// </value>
        public int? UpdatedBy { get; set; }
    }
}
