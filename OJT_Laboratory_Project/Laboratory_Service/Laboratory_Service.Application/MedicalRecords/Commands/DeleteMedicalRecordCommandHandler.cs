using System.Text.Json;
using System.Text.Json.Serialization;
using MediatR;
using Microsoft.Extensions.Logging;
using Laboratory_Service.Application.Interface;
using Laboratory_Service.Application.DTOs.MedicalRecords;
using Laboratory_Service.Domain.Entity;

namespace Laboratory_Service.Application.MedicalRecords.Commands
{
    /// <summary>
    /// Handler for DeleteMedicalRecord
    /// </summary>
    public class DeleteMedicalRecordCommandHandler : IRequestHandler<DeleteMedicalRecordCommand, DeleteMedicalRecordResultDto>
    {
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly ILogger<DeleteMedicalRecordCommandHandler> _logger;

        public DeleteMedicalRecordCommandHandler(
            IMedicalRecordRepository medicalRecordRepository,
            ILogger<DeleteMedicalRecordCommandHandler> logger)
        {
            _medicalRecordRepository = medicalRecordRepository;
            _logger = logger;
        }

        public async Task<DeleteMedicalRecordResultDto> Handle(DeleteMedicalRecordCommand request, CancellationToken cancellationToken)
        {
            // Lấy MedicalRecord
            var medicalRecord = await _medicalRecordRepository.GetByIdAsync(request.MedicalRecordId);
            if (medicalRecord == null)
            {
                _logger.LogWarning("Delete failed. MedicalRecord {Id} not found", request.MedicalRecordId);
                throw new KeyNotFoundException("Medical record not found");
            }

            if (medicalRecord.IsDeleted)
            {
                return new DeleteMedicalRecordResultDto
                {
                    Success = false,
                    Message = "Medical record is already deleted."
                };
            }

            // Kiểm tra xem có TestOrder nào đang Pending/InProgress/Ongoing không
            if (medicalRecord.TestOrders != null && medicalRecord.TestOrders.Any())
            {
                var hasPending = medicalRecord.TestOrders.Any(to =>
                    string.Equals(to.Status, "Pending", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(to.Status, "InProgress", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(to.Status, "Ongoing", StringComparison.OrdinalIgnoreCase)
                );

                if (hasPending)
                {
                    throw new InvalidOperationException("Cannot delete medical record while there are pending or ongoing test orders.");
                }
            }

            // Tạo snapshot lịch sử
            var snapshotObj = new
            {
                medicalRecord.MedicalRecordId,
                medicalRecord.PatientId,
                medicalRecord.CreatedAt,
                medicalRecord.CreatedBy,
                medicalRecord.UpdatedAt,
                medicalRecord.UpdatedBy,
                medicalRecord.IsDeleted
            };

            var jsonOptions = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = false,
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };

            var snapshotJson = JsonSerializer.Serialize(snapshotObj, jsonOptions);
            var history = new MedicalRecordHistory
            {
                MedicalRecordId = medicalRecord.MedicalRecordId,
                SnapshotJson = snapshotJson,
                ChangeSummary = $"Record marked as deleted by {request.DeletedBy}",
                ChangedBy = string.IsNullOrEmpty(request.DeletedBy) ? "unknown" : request.DeletedBy,
                ChangedAt = DateTime.UtcNow
            };

            // Thực hiện soft delete
            medicalRecord.IsDeleted = true;
            medicalRecord.UpdatedAt = DateTime.UtcNow;
            medicalRecord.UpdatedBy = request.DeletedBy ?? medicalRecord.UpdatedBy;

            await _medicalRecordRepository.AddHistoryAsync(history);
            await _medicalRecordRepository.UpdateAsync(medicalRecord);

            _logger.LogInformation("MedicalRecord {Id} soft-deleted by {User}", medicalRecord.MedicalRecordId, history.ChangedBy);

            return new DeleteMedicalRecordResultDto
            {
                Success = true,
                Message = "Medical record soft-deleted successfully."
            };
        }
    }
}
