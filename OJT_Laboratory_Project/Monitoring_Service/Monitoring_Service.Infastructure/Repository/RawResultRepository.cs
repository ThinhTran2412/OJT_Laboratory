using Microsoft.EntityFrameworkCore;
using Monitoring_Service.Application.Interface;
using Monitoring_Service.Domain.Entity;
using Monitoring_Service.Infastructure.Data;

namespace Monitoring_Service.Infastructure.Repository
{
    /// <summary>
    /// Implements the raw result repository.
    /// </summary>
    /// <seealso cref="Monitoring_Service.Application.Interface.IRawResultRepository" />
    public class RawResultRepository : IRawResultRepository
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="RawResultRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public RawResultRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds the backup asynchronous.
        /// </summary>
        /// <param name="data">The data.</param>
        public async Task AddBackupAsync(RawTestResult data)
        {
            await _context.RawResults.AddAsync(data);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Adds the backup range asynchronous.
        /// </summary>
        /// <param name="data">The data.</param>
        public async Task AddBackupRangeAsync(List<RawTestResult> data)
        {
            if (data == null || data.Count == 0)
            {
                return;
            }

            // Giả định rằng AppDbContext có DbSet tên là RawResults
            await _context.RawResults.AddRangeAsync(data);
            await _context.SaveChangesAsync();
        }
        /// <summary>
        /// Existses the asynchronous.
        /// </summary>
        /// <param name="testOrderId">The test order identifier.</param>
        /// <param name="testCode">The test code.</param>
        /// <returns></returns>
        public async Task<bool> ExistsAsync(Guid testOrderId, string testCode)
        {
            return await _context.RawResults
                .AnyAsync(r => r.TestOrderId == testOrderId && r.TestCode == testCode);
        }

    }
}
