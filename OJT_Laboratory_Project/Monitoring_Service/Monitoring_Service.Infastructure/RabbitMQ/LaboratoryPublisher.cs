using Monitoring_Service.Application.DTOs;
using Monitoring_Service.Application.Interface;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Monitoring_Service.Infrastructure.RabbitMQ
{
    /// <summary>
    /// Implements the laboratory publisher.
    /// </summary>
    /// <seealso cref="Monitoring_Service.Application.Interface.ILaboratoryPublisher" />
    /// <seealso cref="System.IDisposable" />
    public class LaboratoryPublisher : ILaboratoryPublisher, IDisposable
    {
        /// <summary>
        /// The connection
        /// </summary>
        private IConnection? _connection;
        /// <summary>
        /// The channel
        /// </summary>
        private IModel? _channel;
        /// <summary>
        /// The queue name
        /// </summary>
        private const string QueueName = "monitoring.to.laboratory.rawresult";
        /// <summary>
        /// The configuration
        /// </summary>
        private readonly IConfiguration _configuration;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<LaboratoryPublisher>? _logger;
        /// <summary>
        /// Lock object for thread safety
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="LaboratoryPublisher"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logger">The logger.</param>
        public LaboratoryPublisher(IConfiguration configuration, ILogger<LaboratoryPublisher>? logger = null)
        {
            _configuration = configuration;
            _logger = logger;
            // Lazy initialization - don't connect in constructor
            // Connection will be established on first use
        }

        /// <summary>
        /// Ensures the connection is established.
        /// </summary>
        private void EnsureConnection()
        {
            if (_connection != null && _connection.IsOpen)
            {
                return;
            }

            lock (_lock)
            {
                if (_connection != null && _connection.IsOpen)
                {
                    return;
                }

                try
                {
                    var hostName = _configuration["RabbitMQ:HostName"] ?? "rabbitmq";
                    var userName = _configuration["RabbitMQ:UserName"] ?? "guest";
                    var password = _configuration["RabbitMQ:Password"] ?? "guest";

                    _logger?.LogInformation("Connecting to RabbitMQ at {HostName}", hostName);

                    var factory = new ConnectionFactory
                    {
                        HostName = hostName,
                        UserName = userName,
                        Password = password,
                        AutomaticRecoveryEnabled = true,
                        NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
                    };

                    // Retry connection with exponential backoff
                    var maxRetries = 5;
                    var retryCount = 0;
                    var delay = TimeSpan.FromSeconds(2);

                    while (retryCount < maxRetries)
                    {
                        try
                        {
                            _connection = factory.CreateConnection();
                            _channel = _connection.CreateModel();

                            _channel.QueueDeclare(queue: QueueName,
                                                  durable: true,
                                                  exclusive: false,
                                                  autoDelete: false,
                                                  arguments: null);

                            _logger?.LogInformation("Successfully connected to RabbitMQ at {HostName}", hostName);
                            return;
                        }
                        catch (Exception ex) when (retryCount < maxRetries - 1)
                        {
                            retryCount++;
                            _logger?.LogWarning(ex, "Failed to connect to RabbitMQ (attempt {RetryCount}/{MaxRetries}). Retrying in {Delay} seconds...", retryCount, maxRetries, delay.TotalSeconds);
                            Thread.Sleep(delay);
                            delay = TimeSpan.FromSeconds(delay.TotalSeconds * 2); // Exponential backoff
                        }
                    }

                    // If all retries failed, throw the last exception
                    throw new InvalidOperationException($"Failed to connect to RabbitMQ after {maxRetries} attempts");
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Failed to establish RabbitMQ connection");
                    throw;
                }
            }
        }

        /// <summary>
        /// Publishes the asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public Task PublishAsync(RawTestResultDTO message)
        {
            try
            {
                EnsureConnection();

                if (_channel == null || !_channel.IsOpen)
                {
                    throw new InvalidOperationException("RabbitMQ channel is not open");
                }

                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
                _channel.BasicPublish(exchange: "",
                                      routingKey: QueueName,
                                      basicProperties: null,
                                      body: body);
                
                _logger?.LogDebug("Published message to queue: {QueueName}", QueueName);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to publish message to RabbitMQ queue: {QueueName}", QueueName);
                throw;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            try
            {
                _channel?.Close();
                _channel?.Dispose();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error closing RabbitMQ channel");
            }

            try
            {
                _connection?.Close();
                _connection?.Dispose();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error closing RabbitMQ connection");
            }
        }
    }
}
