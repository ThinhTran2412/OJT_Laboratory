namespace IAM_Service.Application.DTOs
{
    /// <summary>
    /// Internal DTO representing detailed patient information retrieved from an external service (Laboratory Service).
    /// This DTO acts as the boundary object (ACL) for the IAM_Service application layer.
    /// </summary>
    public class PatientDetailDto
    {
        /// <summary>
        /// Gets or sets the primary key/ID of the patient record in the external system.
        /// </summary>
        public int PatientId { get; set; }

        /// <summary>
        /// Gets or sets the identification number (e.g., Citizen ID, Passport) of the patient.
        /// This is the key linking the user in IAM_Service to the patient in Laboratory_Service.
        /// </summary>
        public string IdentifyNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the full name of the patient.
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date of birth of the patient.
        /// Using DateOnly as per your original structure, but nullable for robustness if external data is missing.
        /// </summary>
        public DateOnly? DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the calculated age of the patient.
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// Gets or sets the gender of the patient.
        /// </summary>
        public string Gender { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the contact phone number of the patient.
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email address of the patient.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the residential address of the patient.
        /// </summary>
        public string Address { get; set; } = string.Empty;
    }
}