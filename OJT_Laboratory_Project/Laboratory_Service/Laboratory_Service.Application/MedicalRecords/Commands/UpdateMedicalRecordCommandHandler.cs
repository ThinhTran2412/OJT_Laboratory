using Laboratory_Service.Application.DTOs.MedicalRecords;
using Laboratory_Service.Application.Interface;
using Laboratory_Service.Domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Laboratory_Service.Application.MedicalRecords.Commands
{
    /// <summary>
    /// Handle UpdateMedicalRecordCommandHandler with patch behavior
    /// </summary>
    public class UpdateMedicalRecordCommandHandler : IRequestHandler<UpdateMedicalRecordCommand, UpdateMedicalRecordResultDto>
    {
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly ILogger<UpdateMedicalRecordCommandHandler> _logger;

        public UpdateMedicalRecordCommandHandler(
            IMedicalRecordRepository medicalRecordRepository,
            ILogger<UpdateMedicalRecordCommandHandler> logger)
        {
            _medicalRecordRepository = medicalRecordRepository;
            _logger = logger;
        }

        public async Task<UpdateMedicalRecordResultDto> Handle(UpdateMedicalRecordCommand request, CancellationToken cancellationToken)
        {
            var medicalRecord = await _medicalRecordRepository.GetByIdAsync(request.MedicalRecordId);

            if (medicalRecord == null)
                throw new KeyNotFoundException("Medical record not found.");

            var patient = medicalRecord.Patient;
            if (patient == null)
                throw new InvalidOperationException("Medical record has no linked patient.");

            var changes = new List<string>();

            void AddChange<T>(string name, T oldValue, T newValue)
            {
                if (!Equals(oldValue, newValue))
                    changes.Add($"{name}: '{oldValue}' -> '{newValue}'");
            }

            var snapshotJson = JsonSerializer.Serialize(patient, new JsonSerializerOptions
            {
                WriteIndented = false,
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            });
            if (!string.IsNullOrEmpty(request.FullName) && request.FullName != "string" && patient.FullName != request.FullName)
            {
                AddChange("FullName", patient.FullName, request.FullName);
                patient.FullName = request.FullName;
            }

            if (request.DateOfBirth != default && patient.DateOfBirth != request.DateOfBirth)
            {
                AddChange("DateOfBirth", patient.DateOfBirth, request.DateOfBirth);
                patient.DateOfBirth = request.DateOfBirth;
            }

            if (!string.IsNullOrEmpty(request.Gender) && request.Gender != "string" && patient.Gender != request.Gender)
            {
                AddChange("Gender", patient.Gender, request.Gender);
                patient.Gender = request.Gender;
            }

            if (!string.IsNullOrEmpty(request.PhoneNumber) && request.PhoneNumber != "string" && patient.PhoneNumber != request.PhoneNumber)
            {
                AddChange("PhoneNumber", patient.PhoneNumber, request.PhoneNumber);
                patient.PhoneNumber = request.PhoneNumber;
            }

            if (!string.IsNullOrEmpty(request.Email) && request.Email != "string" && patient.Email != request.Email)
            {
                AddChange("Email", patient.Email, request.Email);
                patient.Email = request.Email;
            }

            if (!string.IsNullOrEmpty(request.Address) && request.Address != "string" && patient.Address != request.Address)
            {
                AddChange("Address", patient.Address, request.Address);
                patient.Address = request.Address;
            }

            if (!string.IsNullOrEmpty(request.IdentifyNumber) && request.IdentifyNumber != "string" && patient.IdentifyNumber != request.IdentifyNumber)
            {
                AddChange("IdentifyNumber", patient.IdentifyNumber, request.IdentifyNumber);
                patient.IdentifyNumber = request.IdentifyNumber;
            }

            if (changes.Count > 0)
            {
                medicalRecord.UpdatedAt = DateTime.UtcNow;
                medicalRecord.UpdatedBy = request.UpdatedBy;

                var history = new MedicalRecordHistory
                {
                    MedicalRecordId = medicalRecord.MedicalRecordId,
                    SnapshotJson = snapshotJson,
                    ChangeSummary = string.Join("; ", changes),
                    ChangedBy = request.UpdatedBy,
                    ChangedAt = DateTime.UtcNow
                };
                await _medicalRecordRepository.AddHistoryAsync(history);

                await _medicalRecordRepository.UpdateAsync(medicalRecord);
            }

            return new UpdateMedicalRecordResultDto
            {
                Success = true,
                Message = changes.Count == 0 ? "No changes detected." : "Medical record updated successfully."
            };
        }
    }
}
