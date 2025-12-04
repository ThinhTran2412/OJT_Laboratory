using Laboratory_Service.Application.DTOs.MedicalRecords;
using MediatR;
using System.Text.Json.Serialization;

namespace Laboratory_Service.Application.MedicalRecords.Commands
{
    /// <summary>
    /// Create attribute for AddMedicalRecordCommand
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;Laboratory_Service.Application.DTOs.MedicalRecords.CreateMedicalRecordResultDto&gt;" />
    /// <seealso cref="MediatR.IRequest&lt;Laboratory_Service.Application.DTOs.MedicalRecords.CreateMedicalRecordResultDto&gt;" />
    public class AddMedicalRecordCommand : IRequest<CreateMedicalRecordResultDto>
    {

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
        public string? PhoneNumber { get; set; }
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string? Email { get; set; }
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string? Address { get; set; }
        /// <summary>
        /// Gets or sets the identify number.
        /// </summary>
        /// <value>
        /// The identify number.
        /// </value>
        public string IdentifyNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        /// <value>
        /// The created by.
        /// </value>
        [JsonIgnore]
        public string CreatedBy { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets a value indicating whether [create iam user].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [create iam user]; otherwise, <c>false</c>.
        /// </value>
        public bool CreateIAMUser { get; set; } = false; 
    }
}
