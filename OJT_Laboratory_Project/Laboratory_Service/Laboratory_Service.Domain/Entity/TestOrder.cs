using System.ComponentModel.DataAnnotations.Schema;

namespace Laboratory_Service.Domain.Entity
{
    /// <summary>
    /// Create attribyte for class TestOrder
    /// </summary>
    public class TestOrder
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
        public string OrderCode { get; set; } = default!;
        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        public string Priority { get; set; } = "Normal";
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string Status { get; set; } = "Created";
        /// <summary>
        /// Gets or sets the note.
        /// </summary>
        /// <value>
        /// The note.
        /// </value>
        public string? Note { get; set; }


        /// <summary>
        /// Gets or sets the created at.
        /// </summary>
        /// <value>
        /// The created at.
        /// </value>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        /// <value>
        /// The created by.
        /// </value>
        public string CreatedBy { get; set; }
        /// <summary>
        /// Gets or sets the updated at.
        /// </summary>
        /// <value>
        /// The updated at.
        /// </value>
        public DateTime? UpdatedAt { get; set; }
        /// <summary>
        /// Gets or sets the updated by.
        /// </summary>
        /// <value>
        /// The updated by.
        /// </value>
        public string? UpdatedBy { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is deleted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is deleted; otherwise, <c>false</c>.
        /// </value>
        public bool IsDeleted { get; set; } = false;
        /// <summary>
        /// Gets or sets the deleted at.
        /// </summary>
        /// <value>
        /// The deleted at.
        /// </value>
        public DateTime? DeletedAt { get; set; }
        /// <summary>
        /// Gets or sets the deleted by.
        /// </summary>
        /// <value>
        /// The deleted by.
        /// </value>
        public string? DeletedBy { get; set; }


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
        public virtual ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();

        /// <summary>
        /// Gets or sets the medical record identifier.
        /// </summary>
        /// <value>
        /// The medical record identifier.
        /// </value>
        public int MedicalRecordId { get; set; }

        /// <summary>
        /// Navigation property to the medical record
        /// </summary>
        /// <value>
        /// The medical record.
        /// </value>
        public virtual MedicalRecord MedicalRecord { get; set; } = null!;
        /// <summary>
        /// User who performed the test
        /// </summary>
        /// <value>
        /// The run by.
        /// </value>
        public int? RunBy { get; set; }

        /// <summary>
        /// Gets or sets the run date.
        /// </summary>
        /// <value>
        /// The run date.
        /// </value>
        public DateTime RunDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is ai review enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is ai review enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsAiReviewEnabled { get; set; } = false;

        [NotMapped]
        public Dictionary<string, object> TempData { get; set; } = new();

    }
}
