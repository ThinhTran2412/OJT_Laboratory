namespace Simulator.Application.Interface
{
    /// <summary>
    /// Creates the rabbit mq publisher interface
    /// </summary>
    public interface IRabbitMQPublisher
    {
        /// <summary>
        /// Publishes the asynchronous.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        Task<string> PublishAsync(string queueName, object message);
    }
}
