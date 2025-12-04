using Laboratory_Service.Application.Interface;
using Laboratory_Service.Domain.Entity;
using Laboratory_Service.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Laboratory_Service.Infrastructure.Repositories
{
    /// <summary>
    /// Create methods for RawBackupRepository
    /// </summary>
    /// <seealso cref="Laboratory_Service.Application.Interface.IRawBackupRepository" />
    public class RawBackupRepository : IRawBackupRepository
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="RawBackupRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public RawBackupRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds the asynchronous.
        /// </summary>
        /// <param name="rawBackup">The raw backup.</param>
        public async Task AddAsync(RawBackup rawBackup)
        {
            _context.RawBackups.Add(rawBackup);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Gets the by identifier asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<RawBackup?> GetByIdAsync(Guid id)
        {
            return await _context.RawBackups.FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}
