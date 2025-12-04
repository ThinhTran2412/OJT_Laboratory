namespace Laboratory_Service.Application.DTOs.MedicalRecords
{
    /// <summary>
    /// Data Transfer Object for Medical Record
    /// </summary>
    public class MedicalRecordDto
    {
        /// <summary>
        /// Gets or sets the medical record identifier.
        /// </summary>
        public int MedicalRecordId { get; set; }
        /// <summary>
        /// Gets or sets the patient identifier.
        /// </summary>
        public int PatientId { get; set; }
        /// <summary>
        /// Gets or sets the patient name.
        /// </summary>
        public string PatientName { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the test type.
        /// </summary>
        public string TestType { get; set; } = string.Empty;
        /// Gets or sets the test results.
        public string TestResults { get; set; } = string.Empty;
        /// Gets or sets the reference ranges.
        public string ReferenceRanges { get; set; } = string.Empty;
        /// Gets or sets the interpretation.
        public string Interpretation { get; set; } = string.Empty;
        /// Gets or sets the instrument used.
        public string InstrumentUsed { get; set; } = string.Empty;
        /// Gets or sets the batch number.
        public string BatchNumber { get; set; } = string.Empty;
        /// Gets or sets the lot number.
        public string LotNumber { get; set; } = string.Empty;
        /// Gets or sets the clinical notes.
        public string ClinicalNotes { get; set; } = string.Empty;
        /// Gets or sets the error messages.
        public string ErrorMessages { get; set; } = string.Empty;
        /// Gets or sets the test date.
        public DateTime TestDate { get; set; }
        /// Gets or sets the results date.
        public DateTime? ResultsDate { get; set; }
        /// Gets or sets the status.
        public string Status { get; set; } = string.Empty;
        /// Gets or sets the priority.
        public string Priority { get; set; } = string.Empty;
        /// Gets or sets the created at.
        public DateTime CreatedAt { get; set; }
        /// Gets or sets the created by.
        public string CreatedBy { get; set; } = string.Empty;
    }
}