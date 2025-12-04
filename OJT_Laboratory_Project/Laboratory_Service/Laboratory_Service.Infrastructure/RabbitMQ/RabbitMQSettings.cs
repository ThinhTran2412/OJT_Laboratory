namespace Laboratory_Service.Infrastructure.RabbitMQ
{
    /// <summary>
    /// Configuration settings for RabbitMQ.
    /// </summary>
    public static class RabbitMQSettings
    {
        /// <summary>
        /// The host
        /// </summary>
        public const string Host = "localhost";
        /// <summary>
        /// The username
        /// </summary>
        public const string Username = "guest";
        /// <summary>
        /// The password
        /// </summary>
        public const string Password = "guest";

        /// <summary>
        /// The queue raw from monitoring
        /// </summary>
        public const string Queue_Raw_From_Monitoring = "monitoring.to.laboratory.rawresult";
    }
}
