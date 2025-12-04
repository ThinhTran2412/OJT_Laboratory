namespace Laboratory_Service.Application.DTOs.Patients
{
    /// <summary>
    /// create attribute for CreatePatient
    /// </summary>
    public class CreatePatientByIdentityDto
    {
        /// <summary>
        /// Gets or sets the identify number.
        /// </summary>
        /// <value>
        /// The identify number.
        /// </value>
        public string IdentifyNumber { get; set; } = string.Empty;
    }
}
