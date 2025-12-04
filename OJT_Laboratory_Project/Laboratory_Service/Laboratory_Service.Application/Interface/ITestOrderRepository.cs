using Laboratory_Service.Application.DTOs.Pagination;
using Laboratory_Service.Domain.Entity;

namespace Laboratory_Service.Application.Interface
{
    /// <summary>
    /// Create methods for interface TestOrderRepository
    /// </summary>
    public interface ITestOrderRepository
    {
        /// <summary>
        /// Adds the asynchronous.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task AddAsync(TestOrder order, CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets the by identifier asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<TestOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Gets the by identifier for update (without includes to avoid SQL issues).
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<TestOrder?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Gets the AI review enabled status for a test order by ID (lightweight query without includes).
        /// </summary>
        /// <param name="id">The test order identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>True if AI review is enabled, false otherwise. Returns null if test order not found.</returns>
        Task<bool?> GetAiReviewEnabledByIdAsync(Guid id, CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets all by patient identifier asynchronous.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<IEnumerable<TestOrder>> GetAllByPatientIdAsync(int patientId, CancellationToken cancellationToken = default);
        /// <summary>
        /// Saves the changes asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="testOrder">The test order.</param>
        /// <returns></returns>
        Task UpdateAsync(TestOrder testOrder);
        
        /// <summary>
        /// Updates only the status of a test order (lightweight update without navigation properties).
        /// </summary>
        /// <param name="testOrderId">The test order identifier.</param>
        /// <param name="status">The new status.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task UpdateStatusAsync(Guid testOrderId, string status, CancellationToken cancellationToken = default);
        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="testOrder">The test order.</param>
        /// <returns></returns>
        Task DeleteAsync(TestOrder testOrder);

        /// <summary>
        /// Gets test orders with pagination, search, sorting and filtering.
        /// </summary>
        /// <param name="search">Search keyword.</param>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="sortBy">Sort column.</param>
        /// <param name="sortDesc">Sort direction.</param>
        /// <param name="status">Status filter.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        /// Paged result of test orders.
        /// </returns>
        Task<PagedResult<TestOrder>> GetTestOrdersAsync(
            string? search,
            int page,
            int pageSize,
            string? sortBy,
            bool sortDesc,
            string? status,
            CancellationToken cancellationToken);

        /// <summary>
        /// Gets the by identifier with results asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<TestOrder?> GetByIdWithResultsAsync(Guid id, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Gets the by identifier with results only (for AI review operations, without MedicalRecord to avoid SQL issues).
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<TestOrder?> GetByIdWithResultsOnlyAsync(Guid id, CancellationToken cancellationToken = default);

    }
}
