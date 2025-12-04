using Laboratory_Service.Domain.Entity;

namespace Laboratory_Service.Application.Interface
{
    /// <summary>
    /// Create IAiReviewService
    /// </summary>
    public interface IAiReviewService
    {
        /// <summary>
        /// Trains the model asynchronous.
        /// </summary>
        /// <param name="testResults">The test results.</param>
        /// <returns></returns>
        Task TrainModelAsync(IEnumerable<TestResult> testResults);
        /// <summary>
        /// Predicts the asynchronous.
        /// </summary>
        /// <param name="testResult">The test result.</param>
        /// <returns></returns>
        Task<string> PredictAsync(TestResult testResult);
    }
}
