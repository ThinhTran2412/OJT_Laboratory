using Laboratory_Service.Domain.Entity;

namespace Laboratory_Service.Application.Interface
{
    /// <summary>
    /// Repository interface for TestResult operations.
    /// </summary>
    public interface ITestResultRepository
    {
        /// <summary>
        /// Adds a single test result asynchronously.
        /// </summary>
        /// <param name="testResult">The test result to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The added test result.</returns>
        Task<TestResult> AddAsync(TestResult testResult, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds multiple test results asynchronously (bulk insert).
        /// </summary>
        /// <param name="testResults">The list of test results to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The number of test results added.</returns>
        Task<int> AddRangeAsync(List<TestResult> testResults, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all test results by patient identifier with test order information.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Collection of test results with test order information.</returns>
        Task<IEnumerable<TestResult>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Training data: only get TestResults that have ValueNumeric and ResultStatus.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Collection of TestResults for training.</returns>
        Task<IEnumerable<TestResult>> GetTrainingDatasetAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets test results by TestOrderId.
        /// </summary>
        /// <param name="testOrderId">The test order identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of test results for the specified test order.</returns>
        Task<List<TestResult>> GetByTestOrderIdAsync(Guid testOrderId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets test results by a set of test codes (includes related patient data for flagging).
        /// </summary>
        Task<List<TestResult>> GetByTestCodesAsync(IEnumerable<string> testCodes, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates multiple TestResults asynchronously.
        /// </summary>
        /// <param name="testResults">The test results to update.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task UpdateRangeAsync(IEnumerable<TestResult> testResults, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates flag-related fields for the specified test results.
        /// </summary>
        Task UpdateFlagsAsync(IEnumerable<TestResult> testResults, CancellationToken cancellationToken = default);
    }
}
