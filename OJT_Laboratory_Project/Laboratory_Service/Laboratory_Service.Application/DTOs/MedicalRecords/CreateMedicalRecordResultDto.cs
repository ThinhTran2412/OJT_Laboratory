namespace Laboratory_Service.Application.DTOs.MedicalRecords
{
    /// <summary>
    /// DTO reponse for create Medical Record
    /// </summary>
    public class CreateMedicalRecordResultDto
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CreateMedicalRecordResultDto"/> is success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        public bool Success { get; set; } = true;
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; } = "Medical record created successfully.";
    }
}
