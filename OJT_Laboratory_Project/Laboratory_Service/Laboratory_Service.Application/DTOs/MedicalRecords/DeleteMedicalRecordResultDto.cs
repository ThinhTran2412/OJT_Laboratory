namespace Laboratory_Service.Application.DTOs.MedicalRecords
{
    /// <summary>
    /// Result returned after attempting to delete (soft-delete) a medical record.
    /// Contains a success flag and a human-readable message suitable for returning to API clients.
    /// </summary>
    public class DeleteMedicalRecordResultDto
    {
        /// <summary>
        /// True if the delete operation succeeded; false otherwise (for example: record not found or pending orders prevented deletion).
        /// </summary>
        public bool Success { get; set; } = true;

        /// <summary>
        /// Human-readable message describing the result of the delete operation.
        /// Default: "Medical record deleted successfully."
        /// </summary>
        public string Message { get; set; } = "Medical record deleted successfully.";
    }
}