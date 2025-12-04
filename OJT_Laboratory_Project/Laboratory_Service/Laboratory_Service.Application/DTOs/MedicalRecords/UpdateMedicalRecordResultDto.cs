namespace Laboratory_Service.Application.DTOs.MedicalRecords
{
    /// <summary>
    /// Result returned after attempting to update a medical record.
    /// Contains a success flag and a human-readable message suitable for returning to API clients.
    /// </summary>
    public class UpdateMedicalRecordResultDto
    {
        /// <summary>
        /// True if the update operation succeeded; false otherwise (for example: record not found or validation failed).
        /// </summary>
        public bool Success { get; set; } = true;

        /// <summary>
        /// Human-readable message describing the result of the update operation.
        /// Default: "Medical record updated successfully."
        /// </summary>
        public string Message { get; set; } = "Medical record updated successfully.";
    }
}