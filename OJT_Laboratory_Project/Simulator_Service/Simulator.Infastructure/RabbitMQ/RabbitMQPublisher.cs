using System.Text.Json;
using RabbitMQ.Client;
using System.Text;
using Simulator.Application.Interface;

namespace Simulator.Infastructure.RabbitMQ
{
    /// <summary>
    /// Concrete implementation of RabbitMQ publisher
    /// </summary>
    /// <seealso cref="Simulator.Application.Interface.IRabbitMQPublisher" />
    public class RabbitMQPublisher : IRabbitMQPublisher
    {
        /// <summary>
        /// The connection
        /// </summary>
        private readonly IConnection _connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMQPublisher"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public RabbitMQPublisher(IConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// Publishes the asynchronous.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public Task<string> PublishAsync(string queueName, object message)
        {
            using var channel = _connection.CreateModel();
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            channel.BasicPublish(exchange: "", routingKey: queueName, body: body);

            return Task.FromResult(Guid.NewGuid().ToString());
        }
    }
}
