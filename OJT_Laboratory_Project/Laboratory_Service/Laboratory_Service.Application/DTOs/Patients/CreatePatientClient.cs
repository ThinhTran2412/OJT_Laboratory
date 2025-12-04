namespace Laboratory_Service.Application.DTOs.Patients
{
    /// <summary>
    /// Data Transfer Object for creating a new patient record.
    /// This DTO contains the essential information needed when registering a new patient.
    /// </summary>
    public class CreatePatientClient
    {
        /// <summary>
        /// Gets or sets the identification number (e.g., Citizen ID, Passport) of the patient.
        /// This is a unique identifier required for patient registration.
        /// </summary>
        public string IdentifyNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the full name of the patient.
        /// Should include both first name and last name.
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date of birth of the patient.
        /// Used to calculate age and verify patient identity.
        /// </summary>
        public DateOnly DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the gender of the patient.
        /// Typically "Male", "Female", or other gender identifications.
        /// </summary>
        public string Gender { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the contact phone number of the patient.
        /// Used for communication and appointment notifications.
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email address of the patient.
        /// Used for sending test results and other communications.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the residential address of the patient.
        /// Important for home visits or emergency contact purposes.
        /// </summary>
        public string Address { get; set; } = string.Empty;
    }
}