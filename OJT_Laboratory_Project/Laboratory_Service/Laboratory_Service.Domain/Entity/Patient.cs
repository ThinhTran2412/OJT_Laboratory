
 using System.ComponentModel.DataAnnotations;

namespace Laboratory_Service.Domain.Entity
{
    /// <summary>
    /// Represents a patient in the laboratory system
    /// </summary>
    public class Patient
    {
        /// <summary>
        /// Unique identifier for the patient
        /// </summary>
        public int PatientId { get; set; }

        /// <summary>
        /// Patient's identification number (Căn cước công dân)
        /// This links to IAM Service User.IdentifyNumber
        /// </summary>
        public string IdentifyNumber { get; set; } = string.Empty;

        /// <summary>
        /// Patient's full name
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Patient's date of birth
        /// </summary>
        public DateOnly DateOfBirth { get; set; }

        /// <summary>
        /// Patient's gender
        /// </summary>
        public string Gender { get; set; } = string.Empty;

        /// <summary>
        /// Patient's phone number
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Patient's email address
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Patient's address
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Patient's age (calculated from DateOfBirth)
        /// </summary>
        public int Age => CalculateAge();

        /// <summary>
        /// Date when the patient record was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date when the patient record was last updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// User who created the patient record
        /// </summary>
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// Navigation to the single medical record of this patient.
        /// </summary>
        public virtual MedicalRecord MedicalRecord { get; set; } = null!;

        /// <summary>
        /// Calculate age from date of birth
        /// </summary>
        /// <returns>Age in years</returns>
        private int CalculateAge()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - DateOfBirth.Year;
            if (DateOfBirth > today.AddYears(-age)) age--;
            return age;
        }
    }
}
