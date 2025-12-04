namespace Laboratory_Service.Domain.Entity
{
    /// <summary>
    /// Represents a medical record for a patient.
    /// </summary>
    public class MedicalRecord
    {
        /// <summary>
        /// Unique identifier for the medical record.
        /// </summary>
        public int MedicalRecordId { get; set; }

        /// <summary>
        /// Foreign key to the patient (Database relationship).
        /// </summary>
        public int PatientId { get; set; }

        /// <summary>
        /// Navigation property to the patient (direct EF relationship, not via proto).
        /// </summary>
        public virtual Patient Patient { get; set; } = null!;

        /// <summary>
        /// Date when the record was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date when the record was last updated.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// User who created the medical record.
        /// </summary>
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// User who last updated the medical record.
        /// </summary>
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// Indicates if the record is soft-deleted.
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// Navigation property: A medical record can have multiple test orders.
        /// </summary>
        public virtual ICollection<TestOrder> TestOrders { get; set; } = new List<TestOrder>();
    }
}
