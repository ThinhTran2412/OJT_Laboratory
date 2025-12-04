using Simulator.Domain.Entity;

namespace Simulator.Application.Interface
{
    /// <summary>
    /// Creates the event publisher interface
    /// </summary>
    public interface IEventPublisher
    {
        /// <summary>
        /// Publishes the raw test result event asynchronous.
        /// </summary>
        /// <param name="evt">The evt.</param>
        /// <returns></returns>
        Task PublishRawTestResultEventAsync(RawTestResult evt);
    }
}
