using Laboratory_Service.Application.Interface;
using Laboratory_Service.Domain.Entity;
using MediatR;
using System.Net.Http.Json;
using System.Text.Json;

namespace Laboratory_Service.Application.AiReviewForTestOrder.Command
{
    /// <summary>
    /// Create TriggerAiReviewCommandHandler
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;Laboratory_Service.Application.AiReviewForTestOrder.Command.TriggerAiReviewCommand, Laboratory_Service.Domain.Entity.TestOrder&gt;" />
    public class TriggerAiReviewCommandHandler : IRequestHandler<TriggerAiReviewCommand, TestOrder?>
    {
        private readonly ITestOrderRepository _testOrderRepository;
        private readonly ITestResultRepository _testResultRepository;
        private readonly HttpClient _http;

        public TriggerAiReviewCommandHandler(
            ITestOrderRepository testOrderRepository,
            ITestResultRepository testResultRepository)
        {
            _testOrderRepository = testOrderRepository;
            _testResultRepository = testResultRepository;

            _http = new HttpClient
            {
                BaseAddress = new Uri("http://host.docker.internal:8000/review")
            };
        }

        public async Task<TestOrder?> Handle(TriggerAiReviewCommand request, CancellationToken cancellationToken)
        {
            // 1) Load TestOrder
            var testOrder = await _testOrderRepository.GetByIdForUpdateAsync(request.TestOrderId, cancellationToken);
            if (testOrder == null)
                return null;

            if (!testOrder.IsAiReviewEnabled)
                throw new InvalidOperationException("AI review feature is not enabled for this test order.");

            // 2) Load Results
            var testResults = await _testResultRepository.GetByTestOrderIdAsync(request.TestOrderId, cancellationToken);
            if (testResults == null || !testResults.Any())
                throw new InvalidOperationException("Test order has no results to review.");

            testOrder.TestResults = testResults;

            // 3) Build FastAPI request body
            var body = new
            {
                test_order_id = testOrder.TestOrderId.ToString(),
                results = testResults.Select(r => new
                {
                    name = r.Parameter,
                    value = r.ValueNumeric ?? 0,
                    unit = r.Unit ?? ""
                }).ToList(),
                flags = new List<string>(),
                meta = new { }
            };

            // 4) Call FastAPI
            var response = await _http.PostAsJsonAsync("/review", body, cancellationToken);
            if (!response.IsSuccessStatusCode)
                throw new Exception($"AI API Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");

            // 5) Read response JSON
            var json = await response.Content.ReadFromJsonAsync<JsonElement>();

            string predictedStatus = json.TryGetProperty("predicted_status", out var ps)
                ? ps.GetString() ?? "Unknown"
                : "Unknown";

            string aiSummary = json.TryGetProperty("ai_summary", out var sm)
                ? sm.GetString() ?? ""
                : "";

            // Store AI response temporarily inside TestOrder (no DB change)
            testOrder.TempData["ai_summary"] = aiSummary;
            testOrder.TempData["predicted_status"] = predictedStatus;


            // 6) Update each result with the same predictedStatus
            foreach (var result in testOrder.TestResults)
            {
                result.ResultStatus = predictedStatus;
                result.ReviewedByAI = true;
                result.AiReviewedDate = DateTime.UtcNow;
            }

            // Save result table
            await _testResultRepository.UpdateRangeAsync(testOrder.TestResults, cancellationToken);

            // 7) Update TestOrder
            await _testOrderRepository.UpdateStatusAsync(testOrder.TestOrderId, "Reviewed By AI", cancellationToken);
            testOrder.Status = "Reviewed By AI";

            // Reload results
            testOrder.TestResults = await _testResultRepository.GetByTestOrderIdAsync(request.TestOrderId, cancellationToken)
                                  ?? new List<TestResult>();

            return testOrder;
        }
    }


}
