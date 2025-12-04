using System.Linq;
using Laboratory_Service.Application.DTOs.TestOrders;
using Laboratory_Service.Application.Interface;
using MediatR;

namespace Laboratory_Service.Application.TestOrders.Queries
{
    /// <summary>
    /// Handler for getting test results by test order ID
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;Laboratory_Service.Application.TestOrders.Queries.GetTestResultsByTestOrderIdQuery, System.Collections.Generic.List&lt;Laboratory_Service.Application.DTOs.TestOrders.TestResultDto&gt;&gt;" />
    public class GetTestResultsByTestOrderIdQueryHandler : IRequestHandler<GetTestResultsByTestOrderIdQuery, List<TestResultDto>>
    {
        /// <summary>
        /// The test result repository
        /// </summary>
        private readonly ITestResultRepository _testResultRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetTestResultsByTestOrderIdQueryHandler"/> class.
        /// </summary>
        /// <param name="testResultRepository">The test result repository.</param>
        public GetTestResultsByTestOrderIdQueryHandler(ITestResultRepository testResultRepository)
        {
            _testResultRepository = testResultRepository;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<List<TestResultDto>> Handle(GetTestResultsByTestOrderIdQuery request, CancellationToken cancellationToken)
        {
            var testResults = await _testResultRepository.GetByTestOrderIdAsync(request.TestOrderId, cancellationToken);

            var resultDtos = testResults.Select(tr => new TestResultDto
            {
                TestResultId = tr.TestResultId,
                TestCode = tr.TestCode,
                Parameter = tr.Parameter,
                ValueNumeric = tr.ValueNumeric,
                ValueText = tr.ValueText,
                Unit = tr.Unit,
                ReferenceRange = tr.ReferenceRange,
                Instrument = tr.Instrument,
                PerformedDate = tr.PerformedDate,
                ResultStatus = tr.ResultStatus,
                ReviewedBy = tr.ReviewedByUserId,
                ReviewedDate = tr.ReviewedDate,
                // AI Review fields
                ReviewedByAI = tr.ReviewedByAI,
                AiReviewedDate = tr.AiReviewedDate,
                IsConfirmed = tr.IsConfirmed,
                ConfirmedByUserId = tr.ConfirmedByUserId,
                ConfirmedDate = tr.ConfirmedDate
            }).ToList();

            return resultDtos;
        }
    }
}
