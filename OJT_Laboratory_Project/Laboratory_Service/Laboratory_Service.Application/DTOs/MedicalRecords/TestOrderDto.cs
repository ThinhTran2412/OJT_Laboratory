using Laboratory_Service.Application.DTOs.TestOrders;

namespace Laboratory_Service.Application.DTOs.MedicalRecords
{
    /// <summary>
    /// Create TestOrderDto
    /// </summary>
    public class TestOrderDto
    {
        /// <summary>
        /// Gets or sets the test order identifier.
        /// </summary>
        /// <value>
        /// The test order identifier.
        /// </value>
        public Guid TestOrderId { get; set; }
        /// <summary>
        /// Gets or sets the order code.
        /// </summary>
        /// <value>
        /// The order code.
        /// </value>
        public string OrderCode { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        public string Priority { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string Status { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the note.
        /// </summary>
        /// <value>
        /// The note.
        /// </value>
        public string? Note { get; set; }
        /// <summary>
        /// Gets or sets the type of the test.
        /// </summary>
        /// <value>
        /// The type of the test.
        /// </value>
        public string TestType { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the test results.
        /// </summary>
        /// <value>
        /// The test results.
        /// </value>
        public List<TestResultDto> TestResults { get; set; } = new();
        /// <summary>
        /// Gets or sets the run date.
        /// </summary>
        /// <value>
        /// The run date.
        /// </value>
        public DateTime RunDate { get; set; }
        /// <summary>
        /// Gets or sets the run by.
        /// </summary>
        /// <value>
        /// The run by.
        /// </value>
        public int? RunBy { get; set; }
    }
}
