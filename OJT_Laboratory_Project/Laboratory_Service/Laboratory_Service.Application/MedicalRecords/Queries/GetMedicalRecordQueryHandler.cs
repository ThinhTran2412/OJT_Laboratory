using Laboratory_Service.Application.DTOs.MedicalRecords;
using Laboratory_Service.Application.DTOs.TestOrders;
using Laboratory_Service.Application.Interface;
using MediatR;

namespace Laboratory_Service.Application.MedicalRecords.Queries
{
    /// <summary>
    /// Create GetAllMedicalRecordsQueryHandler
    /// Handle Get All Medical Records Query
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;Laboratory_Service.Application.MedicalRecords.Queries.GetAllMedicalRecordsQuery, System.Collections.Generic.List&lt;Laboratory_Service.Application.DTOs.MedicalRecords.MedicalRecordViewDto&gt;&gt;" />
    public class GetAllMedicalRecordsQueryHandler : IRequestHandler<GetAllMedicalRecordsQuery, List<MedicalRecordViewDto>>
    {
        /// <summary>
        /// The repository
        /// </summary>
        private readonly IMedicalRecordRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllMedicalRecordsQueryHandler"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public GetAllMedicalRecordsQueryHandler(IMedicalRecordRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<List<MedicalRecordViewDto>> Handle(GetAllMedicalRecordsQuery request, CancellationToken cancellationToken)
        {
            var records = await _repository.GetAllAsync();

            return records.Select(record => new MedicalRecordViewDto
            {
                MedicalRecordId = record.MedicalRecordId,
                CreatedAt = record.CreatedAt,
                UpdatedAt = record.UpdatedAt,
                CreatedBy = record.CreatedBy,
                UpdatedBy = record.UpdatedBy,
                PatientId = record.PatientId,
                PatientName = record.Patient.FullName,
                DateOfBirth = record.Patient.DateOfBirth,
                Age = record.Patient.Age,
                Gender = record.Patient.Gender,
                Address = record.Patient.Address,
                PhoneNumber = record.Patient.PhoneNumber,
                Email = record.Patient.Email,
                TestOrders = record.TestOrders
                    .Where(t => !t.IsDeleted)
                    .Select(t => new TestOrderDto
                    {
                        TestOrderId = t.TestOrderId,
                        OrderCode = t.OrderCode,
                        Priority = t.Priority,
                        Status = t.Status,
                        TestType = t.TestType,
                        RunDate = t.RunDate,
                        RunBy = t.RunBy,
                        TestResults = t.TestResults
                            .Select(tr => new TestResultDto
                            {
                                TestResultId = tr.TestResultId,
                                TestCode = tr.TestCode,
                                Parameter = tr.Parameter,
                                ValueNumeric = tr.ValueNumeric,
                                ValueText = tr.ValueText,
                                Unit = tr.Unit,
                                ReferenceRange = tr.ReferenceRange,
                                Instrument = tr.Instrument,
                                ResultStatus = tr.ResultStatus,
                                
                                Status = string.IsNullOrWhiteSpace(tr.Flag) ? tr.ResultStatus : tr.Flag,
                                
                                PerformedDate = tr.PerformedDate != default ? tr.PerformedDate : (t.RunDate == default ? null : t.RunDate),
                                PerformedBy = t.RunBy ?? tr.PerformedByUserId,
                                ReviewedBy = tr.ReviewedByUserId,
                                ReviewedDate = tr.ReviewedDate,
                                // AI Review fields
                                ReviewedByAI = tr.ReviewedByAI,
                                AiReviewedDate = tr.AiReviewedDate,
                                IsConfirmed = tr.IsConfirmed,
                                ConfirmedByUserId = tr.ConfirmedByUserId,
                                ConfirmedDate = tr.ConfirmedDate
                            }).ToList()
                    }).ToList()
            }).ToList();
        }
    }
}
