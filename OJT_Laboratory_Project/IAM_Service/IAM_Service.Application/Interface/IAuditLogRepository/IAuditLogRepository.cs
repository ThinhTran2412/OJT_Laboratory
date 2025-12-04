using IAM_Service.Domain.Entity;

namespace IAM_Service.Application.Interface.IAuditLogRepository
{
    /// <summary>
    /// create methods for interface AuditLogRepository
    /// </summary>
    public interface IAuditLogRepository
    {
        /// <summary>
        /// Gets all asynchronous.
        /// </summary>
        /// <returns></returns>
        Task<List<AuditLog>> GetAllAsync();
        Task AddAsync(AuditLog log, CancellationToken cancellationToken = default);
    }
}
