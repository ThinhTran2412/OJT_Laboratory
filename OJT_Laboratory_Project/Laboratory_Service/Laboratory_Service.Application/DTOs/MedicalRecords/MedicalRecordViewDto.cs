namespace Laboratory_Service.Application.DTOs.MedicalRecords
{
    /// <summary>
    /// Create MedicalRecordViewDto
    /// </summary>
    public class MedicalRecordViewDto
    {
        /// <summary>
        /// Gets or sets the medical record identifier.
        /// </summary>
        /// <value>
        /// The medical record identifier.
        /// </value>
        public int MedicalRecordId { get; set; }
        /// <summary>
        /// Gets or sets the created at.
        /// </summary>
        /// <value>
        /// The created at.
        /// </value>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// Gets or sets the updated at.
        /// </summary>
        /// <value>
        /// The updated at.
        /// </value>
        public DateTime? UpdatedAt { get; set; }
        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        /// <value>
        /// The created by.
        /// </value>
        public string CreatedBy { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the updated by.
        /// </summary>
        /// <value>
        /// The updated by.
        /// </value>
        public string UpdatedBy { get; set; } = string.Empty;

        // Patient info
        /// <summary>
        /// Gets or sets the patient identifier.
        /// </summary>
        /// <value>
        /// The patient identifier.
        /// </value>
        public int PatientId { get; set; }
        /// <summary>
        /// Gets or sets the name of the patient.
        /// </summary>
        /// <value>
        /// The name of the patient.
        /// </value>
        public string PatientName { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the date of birth.
        /// </summary>
        /// <value>
        /// The date of birth.
        /// </value>
        public DateOnly DateOfBirth { get; set; }
        /// <summary>
        /// Gets or sets the age.
        /// </summary>
        /// <value>
        /// The age.
        /// </value>
        public int Age { get; set; }
        /// <summary>
        /// Gets or sets the gender.
        /// </summary>
        /// <value>
        /// The gender.
        /// </value>
        public string Gender { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string Address { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        /// <value>
        /// The phone number.
        /// </value>
        public string PhoneNumber { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string? Email { get; set; }

        // TestOrders
        /// <summary>
        /// Gets or sets the test orders.
        /// </summary>
        /// <value>
        /// The test orders.
        /// </value>
        public List<TestOrderDto> TestOrders { get; set; } = new();
    }
}
