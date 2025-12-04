using Simulator.Application.DTOs;
using Simulator.Domain.Entity;

namespace Simulator.Application.Interface
{
    /// <summary>
    /// Creates the raw test result repository interface
    /// </summary>
    public interface IRawTestResultRepository
    {
        /// <summary>
        /// Adds the range asynchronous.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <returns></returns>
        Task AddRangeAsync(List<RawTestResultDTO> results);
        /// <summary>
        /// Gets the results by order identifier asynchronous.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns></returns>
        Task<List<RawTestResult>> GetResultsByOrderIdAsync(Guid orderId);
        /// <summary>
        /// Gets the unsent results asynchronous.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<RawTestResultDTO>> GetUnsentResultsAsync();
        /// <summary>
        /// Marks as sent asynchronous.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns></returns>
        Task MarkAsSentAsync(Guid orderId);
    }
}
