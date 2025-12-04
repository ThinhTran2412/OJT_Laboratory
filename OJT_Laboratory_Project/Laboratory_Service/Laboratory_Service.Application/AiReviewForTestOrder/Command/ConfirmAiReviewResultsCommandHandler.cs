using Laboratory_Service.Application.Interface;
using Laboratory_Service.Domain.Entity;
using MediatR;

namespace Laboratory_Service.Application.AiReviewForTestOrder.Command
{
    /// <summary>
    /// Create ConfirmAiReviewResultsCommandHandler
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;Laboratory_Service.Application.AiReviewForTestOrder.Command.ConfirmAiReviewResultsCommand, Laboratory_Service.Domain.Entity.TestOrder&gt;" />
    public class ConfirmAiReviewResultsCommandHandler : IRequestHandler<ConfirmAiReviewResultsCommand, TestOrder?>
    {
        /// <summary>
        /// The test order repository
        /// </summary>
        private readonly ITestOrderRepository _testOrderRepository;
        /// <summary>
        /// The test result repository
        /// </summary>
        private readonly ITestResultRepository _testResultRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfirmAiReviewResultsCommandHandler"/> class.
        /// </summary>
        /// <param name="testOrderRepository">The test order repository.</param>
        /// <param name="testResultRepository">The test result repository.</param>
        public ConfirmAiReviewResultsCommandHandler(
            ITestOrderRepository testOrderRepository,
            ITestResultRepository testResultRepository)
        {
            _testOrderRepository = testOrderRepository;
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
        /// <exception cref="System.InvalidOperationException">
        /// Test order has not been reviewed by AI. Cannot confirm results.
        /// or
        /// Test order has no results to confirm.
        /// or
        /// No AI-reviewed results found to confirm, or all results have already been confirmed.
        /// </exception>
        public async Task<TestOrder?> Handle(ConfirmAiReviewResultsCommand request, CancellationToken cancellationToken)
        {
            // Query TestOrder separately to avoid SQL issues with Include
            var testOrder = await _testOrderRepository.GetByIdForUpdateAsync(request.TestOrderId, cancellationToken);
            if (testOrder == null)
            {
                return null;
            }

            // AC05: Validate that AI review has been performed
            if (testOrder.Status != "Reviewed By AI")
            {
                throw new InvalidOperationException("Test order has not been reviewed by AI. Cannot confirm results.");
            }

            // Query TestResults separately to avoid SQL issues with Include
            var testResults = await _testResultRepository.GetByTestOrderIdAsync(request.TestOrderId, cancellationToken);
            if (testResults == null || !testResults.Any())
            {
                throw new InvalidOperationException("Test order has no results to confirm.");
            }
            
            // Assign TestResults to TestOrder for processing
            testOrder.TestResults = testResults;

            var aiReviewedResults = testOrder.TestResults.Where(r => r.ReviewedByAI && !r.IsConfirmed).ToList();
            if (!aiReviewedResults.Any())
            {
                throw new InvalidOperationException("No AI-reviewed results found to confirm, or all results have already been confirmed.");
            }

            // AC05: Mark as confirmed by user
            foreach (var result in aiReviewedResults)
            {
                result.IsConfirmed = true;
                result.ConfirmedByUserId = request.ConfirmedByUserId;
                result.ConfirmedDate = DateTime.UtcNow;
            }

            // Save results
            await _testResultRepository.UpdateRangeAsync(aiReviewedResults, cancellationToken);
            
            // Reload TestResults from database to ensure we have the latest data
            var updatedResults = await _testResultRepository.GetByTestOrderIdAsync(request.TestOrderId, cancellationToken);
            testOrder.TestResults = updatedResults ?? new List<TestResult>();

            // Check if all AI-reviewed results have been confirmed
            // If all AI-reviewed results are confirmed, update TestOrder status to "Completed"
            var allAiReviewedResults = testOrder.TestResults.Where(r => r.ReviewedByAI).ToList();
            var allConfirmed = allAiReviewedResults.Any() && allAiReviewedResults.All(r => r.IsConfirmed);
            
            if (allConfirmed)
            {
                // All AI-reviewed results have been confirmed, update TestOrder status to "Completed"
                await _testOrderRepository.UpdateStatusAsync(request.TestOrderId, "Completed", cancellationToken);
                
                // Reload TestOrder to get updated status
                testOrder = await _testOrderRepository.GetByIdForUpdateAsync(request.TestOrderId, cancellationToken);
                if (testOrder != null)
                {
                    // Ensure status is set correctly
                    testOrder.Status = "Completed";
                    testOrder.TestResults = updatedResults ?? new List<TestResult>();
                }
            }

            return testOrder;
        }
    }
}
