using Monitoring_Service.Domain.Entity;

namespace Monitoring_Service.Application.Interface
{
    /// <summary>
    /// Creates the raw result repository.
    /// </summary>
    public interface IRawResultRepository
    {
        /// <summary>
        /// Adds the backup asynchronous.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        Task AddBackupAsync(RawTestResult data);
        /// <summary>
        /// Adds the backup range asynchronous.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        Task AddBackupRangeAsync(List<RawTestResult> data);
        /// <summary>
        /// Existses the asynchronous.
        /// </summary>
        /// <param name="testOrderId">The test order identifier.</param>
        /// <param name="testCode">The test code.</param>
        /// <returns></returns>
        Task<bool> ExistsAsync(Guid testOrderId, string testCode);

    }
}
