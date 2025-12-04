using Laboratory_Service.Application.DTOs.MedicalRecords;
using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Laboratory_Service.Application.MedicalRecords.Commands
{
    /// <summary>
    /// Command to update the medical record (primarily patient info).
    /// </summary>
    public class UpdateMedicalRecordCommand : IRequest<UpdateMedicalRecordResultDto>
    {
        /// <summary>
        /// MedicalRecordId to update.
        /// </summary>
        public int MedicalRecordId { get; set; }

        // ------------ Patient Fields to Update ------------

        /// <summary>
        /// Patient full name.
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Patient date of birth.
        /// </summary>
        public DateOnly DateOfBirth { get; set; }

        /// <summary>
        /// Patient gender.
        /// </summary>
        public string Gender { get; set; } = string.Empty;

        /// <summary>
        /// Phone number.
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Email.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Address.
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// Identify number (CMND/CCCD).
        /// </summary>
        public string IdentifyNumber { get; set; } = string.Empty;

        // ------------ Metadata ------------

        /// <summary>
        /// Who performed the update.
        /// </summary>
        [JsonIgnore]
        public string UpdatedBy { get; set; } = string.Empty;
    }
}
