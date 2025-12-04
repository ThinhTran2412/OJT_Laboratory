using RabbitMQ.Client;

namespace Simulator.Infastructure.RabbitMQ
{
    /// <summary>
    /// Configures the RabbitMQ.
    /// </summary>
    public class RabbitMQConfig
    {
        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns></returns>
        public static IConnection GetConnection(RabbitMQSettings settings)
        {
            var factory = new ConnectionFactory()
            {
                HostName = string.IsNullOrWhiteSpace(settings.HostName) ? "localhost" : settings.HostName,
                UserName = string.IsNullOrWhiteSpace(settings.UserName) ? "guest" : settings.UserName,
                Password = string.IsNullOrWhiteSpace(settings.Password) ? "guest" : settings.Password
            };

            return factory.CreateConnection(); 
        }
    }

}
