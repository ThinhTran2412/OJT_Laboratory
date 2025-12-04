using Laboratory_Service.Application.Interface;
using Laboratory_Service.Domain.Entity;
using Laboratory_Service.Infrastructure.Data;

namespace Laboratory_Service.Infrastructure.Repositories
{
    /// <summary>
    /// Implement method form IEventLogService
    /// </summary>
    /// <seealso cref="Laboratory_Service.Application.Interface.IEventLogService" />
    public class EventLogRepository : IEventLogService
    {
        /// <summary>
        /// The database context
        /// </summary>
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventLogRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public EventLogRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Creates the asynchronous.
        /// </summary>
        /// <param name="log">The log.</param>
        public async Task CreateAsync(EventLog log)
        {
            await _dbContext.EventLogs.AddAsync(log);
            await _dbContext.SaveChangesAsync();
        }
    }
}
