using Laboratory_Service.Domain.Entity;

namespace Laboratory_Service.Application.Interface
{
    /// <summary>
    /// Create methods for interface RawBackupRepository
    /// </summary>
    public interface IRawBackupRepository
    {
        /// <summary>
        /// Adds the asynchronous.
        /// </summary>
        /// <param name="rawBackup">The raw backup.</param>
        /// <returns></returns>
        Task AddAsync(RawBackup rawBackup);
        /// <summary>
        /// Gets the by identifier asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Task<RawBackup?> GetByIdAsync(Guid id);
    }
}
