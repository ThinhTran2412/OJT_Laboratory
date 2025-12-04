using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Laboratory_Service.Application.Test_Result;

namespace Laboratory_Service.Infrastructure.RabbitMQ
{
    /// <summary>
    /// Create RabbitMQ consumer for Raw Test Results
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.Hosting.BackgroundService" />
    public class RabbitMQRawResultConsumer : BackgroundService
    {
        /// <summary>
        /// The service provider
        /// </summary>
        private readonly IServiceProvider _serviceProvider;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<RabbitMQRawResultConsumer> _logger;
        /// <summary>
        /// The configuration
        /// </summary>
        private readonly IConfiguration _configuration;
        /// <summary>
        /// The connection
        /// </summary>
        private IConnection? _connection;
        /// <summary>
        /// The channel
        /// </summary>
        private IModel? _channel;


        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMQRawResultConsumer"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="configuration">The configuration.</param>
        public RabbitMQRawResultConsumer(IServiceProvider serviceProvider, ILogger<RabbitMQRawResultConsumer> logger, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;

            try
            {
                // Read RabbitMQ config from configuration (environment variables or appsettings)
                var hostName = _configuration["RabbitMQ:HostName"] ?? Environment.GetEnvironmentVariable("RabbitMQ__HostName") ?? RabbitMQSettings.Host;
                var userName = _configuration["RabbitMQ:UserName"] ?? Environment.GetEnvironmentVariable("RabbitMQ__UserName") ?? RabbitMQSettings.Username;
                var password = _configuration["RabbitMQ:Password"] ?? Environment.GetEnvironmentVariable("RabbitMQ__Password") ?? RabbitMQSettings.Password;

                var factory = new ConnectionFactory
                {
                    HostName = hostName,
                    UserName = userName,
                    Password = password
                };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                // Register connection event handlers
                _connection.ConnectionShutdown += (sender, args) =>
                {
                    _logger.LogWarning("RabbitMQ connection shutdown. Reason: {Reason}, ReplyText: {ReplyText}", args.ReplyCode, args.ReplyText);
                };

                _channel.ModelShutdown += (sender, args) =>
                {
                    _logger.LogWarning("RabbitMQ channel shutdown. Reason: {Reason}, ReplyText: {ReplyText}", args.ReplyCode, args.ReplyText);
                };

                _channel.QueueDeclare(
                    queue: RabbitMQSettings.Queue_Raw_From_Monitoring,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );
                
                _logger.LogInformation("RabbitMQ Consumer connected and queue '{QueueName}' declared. Host: {Host}, Username: {Username}", 
                    RabbitMQSettings.Queue_Raw_From_Monitoring, hostName, userName);
            }
            catch (Exception ex)
            {
                var errorHostName = _configuration["RabbitMQ:HostName"] ?? Environment.GetEnvironmentVariable("RabbitMQ__HostName") ?? RabbitMQSettings.Host;
                _logger.LogError(ex, "FATAL ERROR: Could not connect to RabbitMQ Host: {HostName}. Consumer will not start processing messages.", errorHostName);
            }
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
        /// See <see href="https://learn.microsoft.com/dotnet/core/extensions/workers">Worker Services in .NET</see> for implementation guidelines.
        /// </remarks>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_channel == null)
            {
                _logger.LogCritical("RabbitMQ channel is null. Aborting ExecuteAsync.");
                return;
            }

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (ch, ea) =>
            {
                var rawJson = Encoding.UTF8.GetString(ea.Body.Span);
                _logger.LogInformation("Received message. DeliveryTag: {DeliveryTag}, Size: {Length} bytes.", ea.DeliveryTag, rawJson.Length);

                using var scope = _serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                try
                {
                    // Send command to handler, which includes SaveChanges
                    await mediator.Send(new SaveRawTestResultCommand(rawJson), stoppingToken);

                    // SUCCESS: Acknowledge the message
                    _channel.BasicAck(ea.DeliveryTag, false);
                    _logger.LogInformation("Successfully processed and acknowledged message for RawBackup. DeliveryTag: {DeliveryTag}", ea.DeliveryTag);
                }
                catch (Exception ex)
                {
                    // FAILURE: Log the detailed error
                    _logger.LogError(ex, "PROCESSING ERROR: Failed to process or save RawBackup to DB. DeliveryTag: {DeliveryTag}, Raw JSON length: {Length} bytes", ea.DeliveryTag, rawJson.Length);

                    // Reject the message. 'requeue: false' means it won't be put back on the queue immediately, 
                    // preventing rapid retry loops on persistent errors (like DB issues).
                    _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);
                    _logger.LogWarning("Message negatively acknowledged (BasicNack) and not requeued. DeliveryTag: {DeliveryTag}", ea.DeliveryTag);
                }
            };

            // Start consuming
            _channel.BasicConsume(
                queue: RabbitMQSettings.Queue_Raw_From_Monitoring,
                autoAck: false, // Explicit acknowledgment is required
                consumer: consumer
            );

            _logger.LogInformation("RabbitMQ Consumer started listening on queue: {QueueName}", RabbitMQSettings.Queue_Raw_From_Monitoring);

            // Keep the service running until cancellation is requested
            // This is critical - without this, the BackgroundService will stop immediately
            try
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("RabbitMQ Consumer is being stopped (cancellation requested).");
            }
        }

        // ADDED: Override StopAsync to ensure cleanup
        /// <summary>
        /// Stops the asynchronous.
        /// </summary>
        /// <param name="stoppingToken">The stopping token.</param>
        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("RabbitMQ Consumer is stopping.");
            _channel?.Close();
            _connection?.Close();
            _channel?.Dispose();
            _connection?.Dispose();
            await base.StopAsync(stoppingToken);
        }
    }
}
