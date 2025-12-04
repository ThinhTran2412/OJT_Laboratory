using Laboratory_Service.Application.DTOs.FlaggingConfig;
using Laboratory_Service.Domain.Entity;

namespace Laboratory_Service.Application.Interface
{
    /// <summary>
    /// Repository interface for FlaggingConfig operations.
    /// </summary>
    public interface IFlaggingConfigRepository
    {
        /// <summary>
        /// Gets the active flagging configuration for a specific test code and gender.
        /// </summary>
        /// <param name="testCode">The test code (e.g., "WBC", "Hb", "RBC").</param>
        /// <param name="gender">The gender ("Male", "Female", or null for both).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The active flagging configuration, or null if not found.</returns>
        Task<FlaggingConfig?> GetActiveConfigAsync(string testCode, string? gender, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all active flagging configurations.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of all active flagging configurations.</returns>
        Task<List<FlaggingConfig>> GetAllActiveConfigsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Inserts or updates a collection of flagging configurations atomically.
        /// </summary>
        Task UpsertConfigsAsync(IEnumerable<FlaggingConfigUpsertItemDto> configs, CancellationToken cancellationToken = default);
        Task<FlaggingConfig?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}

