
using IAM_Service.Application.Interface.IAuditLogRepository;
using IAM_Service.Domain.Entity;
using IAM_Service.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IAM_Service.Infrastructure.Repositories
{
    /// <summary>
    /// Implement methods from IAuditLogRepository
    /// </summary>
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly AppDbContext _context;

        public AuditLogRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new audit log record to the database.
        /// </summary>
        public async Task AddAsync(AuditLog log, CancellationToken cancellationToken = default)
        {
            await _context.AuditLogs.AddAsync(log, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            _context.Entry(log).State = EntityState.Detached; // ✅ đảm bảo luôn insert mới
        }

        /// <summary>
        /// Gets all audit logs (for Event Logs page)
        /// </summary>
        public async Task<List<AuditLog>> GetAllAsync()
        {
            return await _context.AuditLogs
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }
    }
}
