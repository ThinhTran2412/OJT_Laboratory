using Monitoring_Service.Application.DTOs;

namespace Monitoring_Service.Application.Interface
{
    /// <summary>
    /// Creates the laboratory publisher.
    /// </summary>
    public interface ILaboratoryPublisher
    {
        /// <summary>
        /// Publishes the asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        Task PublishAsync(RawTestResultDTO message);
    }
}
