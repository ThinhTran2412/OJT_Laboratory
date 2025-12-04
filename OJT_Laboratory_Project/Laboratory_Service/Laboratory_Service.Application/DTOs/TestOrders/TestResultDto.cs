namespace Laboratory_Service.Application.DTOs.TestOrders
{
    /// <summary>
    /// 
    /// </summary>
    public class TestResultDto
    {
        /// <summary>
        /// Gets or sets the test result identifier.
        /// </summary>
        /// <value>
        /// The test result identifier.
        /// </value>
        public int TestResultId { get; set; }
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
        /// Gets or sets the status (Low/Normal/High).
        /// Match với Status trong RawResultItemDTO của Simulator.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string Status { get; set; } = string.Empty;
        
        // NOTE: Các fields sau đây sẽ được set từ root level (Instrument, PerformedDate) hoặc từ TestOrder
        // Không cần trong message từ Simulator, nhưng vẫn giữ lại để tương thích với entity
        
        /// <summary>
        /// Gets or sets the instrument.
        /// Will be set from root level Instrument.
        /// </summary>
        /// <value>
        /// The instrument.
        /// </value>
        public string Instrument { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the result status (Pending | Completed | Reviewed).
        /// Will be set to "Pending" by default when received from Simulator.
        /// </summary>
        /// <value>
        /// The result status.
        /// </value>
        public string ResultStatus { get; set; } = "Pending";
        
        /// <summary>
        /// Gets or sets the performed by user identifier (from TestOrder RunBy).
        /// </summary>
        /// <value>
        /// The performed by user identifier.
        /// </value>
        public int? PerformedBy { get; set; }
        
        /// <summary>
        /// Gets or sets the performed date (from root level PerformedDate).
        /// </summary>
        /// <value>
        /// The performed date.
        /// </value>
        public DateTime? PerformedDate { get; set; }
        
        /// <summary>
        /// Gets or sets the reviewed by user identifier.
        /// </summary>
        /// <value>
        /// The reviewed by user identifier.
        /// </value>
        public int? ReviewedBy { get; set; }
        
        /// <summary>
        /// Gets or sets the reviewed date.
        /// </summary>
        /// <value>
        /// The reviewed date.
        /// </value>
        public DateTime? ReviewedDate { get; set; }

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
    }
}
