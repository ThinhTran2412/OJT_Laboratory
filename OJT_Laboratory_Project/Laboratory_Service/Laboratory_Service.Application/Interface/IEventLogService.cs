using Laboratory_Service.Domain.Entity;

namespace Laboratory_Service.Application.Interface
{
    /// <summary>
    /// Create methods for interface event log
    /// </summary>
    public interface IEventLogService
    {
        /// <summary>
        /// Creates the asynchronous.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <returns></returns>
        Task CreateAsync(EventLog log);
    }
}
