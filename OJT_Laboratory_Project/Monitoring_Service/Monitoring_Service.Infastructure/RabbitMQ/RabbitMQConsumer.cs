using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using MediatR;
using Monitoring_Service.Application.HandleRawTestResult.Command;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Monitoring_Service.Infastructure.RabbitMQ
{
    /// <summary>
    /// Creates the rabbit mq consumer.
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.Hosting.BackgroundService" />
    public class RabbitMQConsumer : BackgroundService
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
        /// The service provider
        /// </summary>
        private readonly IServiceProvider _serviceProvider;
        /// <summary>
        /// The configuration
        /// </summary>
        private readonly IConfiguration _configuration;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<RabbitMQConsumer> _logger;

        /// <summary>
        /// The queue name
        /// </summary>
        private const string QueueName = "raw_test_result_queue";

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMQConsumer"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logger">The logger.</param>
        public RabbitMQConsumer(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<RabbitMQConsumer> logger)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// This method is called when the <see cref="T:Microsoft.Extensions.Hosting.IHostedService" /> starts. The implementation should return a task that represents
        /// the lifetime of the long running operation(s) being performed.
        /// </summary>
        /// <param name="stoppingToken">Triggered when <see cref="M:Microsoft.Extensions.Hosting.IHostedService.StopAsync(System.Threading.CancellationToken)" /> is called.</param>
        /// <returns>
        /// A <see cref="T:System.Threading.Tasks.Task" /> that represents the long running operations.
        /// </returns>
        /// <remarks>
        /// See <see href="https://docs.microsoft.com/dotnet/core/extensions/workers">Worker Services in .NET</see> for implementation guidelines.
        /// </remarks>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Get RabbitMQ configuration
            var hostName = _configuration["RabbitMQ:HostName"];
            
            // Skip RabbitMQ if not configured (e.g., in Production without RabbitMQ service)
            if (string.IsNullOrEmpty(hostName))
            {
                _logger.LogWarning("RabbitMQ:HostName is not configured. RabbitMQ consumer will not start.");
                return;
            }

            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = hostName,
                    UserName = _configuration["RabbitMQ:UserName"] ?? "guest",
                    Password = _configuration["RabbitMQ:Password"] ?? "guest"
                };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.QueueDeclare(
                    queue: QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                var consumer = new EventingBasicConsumer(_channel);

                consumer.Received += async (model, ea) =>
                {
                    var message = Encoding.UTF8.GetString(ea.Body.ToArray());

                    using var scope = _serviceProvider.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                    // Gửi message sang Handler để xử lý
                    await mediator.Send(new HandleRawTestResultCommand(message), stoppingToken);

                    // Xác nhận đã xử lý xong
                    _channel?.BasicAck(ea.DeliveryTag, false);
                };

                _channel.BasicConsume(
                    queue: QueueName,
                    autoAck: false,
                    consumer: consumer
                );

                _logger.LogInformation("RabbitMQ consumer started successfully on queue: {QueueName}", QueueName);

                // Wait until cancellation is requested
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start RabbitMQ consumer. Error: {Message}", ex.Message);
                // Don't throw - allow service to start without RabbitMQ
            }
        }

        /// <summary>
        /// </summary>
        /// <inheritdoc />
        public override void Dispose()
        {
            try
            {
                _channel?.Close();
                _connection?.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing RabbitMQ connection: {Message}", ex.Message);
            }
            finally
            {
                _channel?.Dispose();
                _connection?.Dispose();
                base.Dispose();
            }
        }
    }
}
