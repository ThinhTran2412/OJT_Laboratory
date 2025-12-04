using Laboratory_Service.Application.DTOs.Patients;
using MediatR;

namespace Laboratory_Service.Application.Patients.Commands
{
    /// <summary>
    /// create attribute for CreatePatientByIdentityCommand
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;Laboratory_Service.Application.DTOs.Patients.PatientDto&gt;" />
    /// <seealso cref="MediatR.IRequest&lt;Laboratory_Service.Application.DTOs.Patients.PatientDto&gt;" />
    public class CreatePatientByIdentityCommand : IRequest<PatientDto>
    {
        /// <summary>
        /// Gets or sets the identify number.
        /// </summary>
        /// <value>
        /// The identify number.
        /// </value>
        public string IdentifyNumber { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        /// <value>
        /// The full name.
        /// </value>
        public string FullName { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the date of birth.
        /// </summary>
        /// <value>
        /// The date of birth.
        /// </value>
        public DateOnly DateOfBirth { get; set; }
        /// <summary>
        /// Gets or sets the gender.
        /// </summary>
        /// <value>
        /// The gender.
        /// </value>
        public string Gender { get; set; } = string.Empty;
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
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string Address { get; set; } = string.Empty;
    }
}
