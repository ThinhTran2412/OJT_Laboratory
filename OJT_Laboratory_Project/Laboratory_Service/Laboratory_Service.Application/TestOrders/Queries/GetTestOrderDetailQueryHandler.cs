using System.Linq;
using Laboratory_Service.Application.DTOs.TestOrders;
using Laboratory_Service.Application.Interface;
using MediatR;
using System.Linq;

namespace Laboratory_Service.Application.TestOrders.Queries
{
    /// <summary>
    /// Handler for getting test order detail
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;Laboratory_Service.Application.TestOrders.Queries.GetTestOrderDetailQuery, Laboratory_Service.Application.DTOs.TestOrders.TestOrderDetailDto&gt;" />
    public class GetTestOrderDetailQueryHandler : IRequestHandler<GetTestOrderDetailQuery, TestOrderDetailDto>
    {

        /// <summary>
        /// The test order repository
        /// </summary>
        private readonly ITestOrderRepository _testOrderRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetTestOrderDetailQueryHandler"/> class.
        /// </summary>
        /// <param name="testOrderRepository">The test order repository.</param>
        public GetTestOrderDetailQueryHandler(ITestOrderRepository testOrderRepository)
        {
            _testOrderRepository = testOrderRepository;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<TestOrderDetailDto?> Handle(GetTestOrderDetailQuery request, CancellationToken cancellationToken)
        {
            // 1) Query repository: get test order with related entities (MedicalRecord, Patient, TestResults)
            // Use GetByIdWithResultsAsync to include TestResults
            var order = await _testOrderRepository.GetByIdWithResultsAsync(request.TestOrderId, cancellationToken);

            // 2) If not found, return null (controller will map to NotFound)
            if (order == null || order.IsDeleted)
            {
                return null;
            }

            // 3) Map entity -> DTO. Patient information comes from TestOrder -> MedicalRecord -> Patient.
            var dto = new TestOrderDetailDto
            {
                TestOrderId = order.TestOrderId,
                PatientId = order.MedicalRecord?.PatientId ?? 0,

                // Patient Information from MedicalRecord -> Patient
                PatientName = order.MedicalRecord?.Patient?.FullName ?? string.Empty,
                Age = order.MedicalRecord?.Patient?.Age ?? 0,
                Gender = order.MedicalRecord?.Patient?.Gender ?? string.Empty,
                PhoneNumber = order.MedicalRecord?.Patient?.PhoneNumber ?? string.Empty,
                IdentifyNumber = order.MedicalRecord?.Patient?.IdentifyNumber ?? string.Empty,

                // Test Order Details
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                RunDate = order.RunDate,
                TestType = order.TestType,
                Priority = order.Priority,
                Note = order.Note
            };
            
            // 4) Map TestResults (list) - including all fields from TestResult entity
            if (order.TestResults != null && order.TestResults.Any())
            {
                dto.TestResults = order.TestResults
                    .OrderByDescending(tr => tr.TestResultId)
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
                        
                        PerformedDate = tr.PerformedDate != default ? tr.PerformedDate : (order.RunDate == default ? null : order.RunDate),
                        
                        PerformedBy = order.RunBy ?? tr.PerformedByUserId,

                        
                        ReviewedBy = tr.ReviewedByUserId,
                        ReviewedDate = tr.ReviewedDate,
                        // AI Review fields
                        ReviewedByAI = tr.ReviewedByAI,
                        AiReviewedDate = tr.AiReviewedDate,
                        IsConfirmed = tr.IsConfirmed,
                        ConfirmedByUserId = tr.ConfirmedByUserId,
                        ConfirmedDate = tr.ConfirmedDate
                    })
                    .ToList();
            }
            else
            {
                dto.Message = "No test results available for this order.";
            }

            return dto;
        }
    }
}
