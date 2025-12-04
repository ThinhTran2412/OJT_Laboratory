namespace IAM_Service.Infrastructure.Configuration
{
    // Represents the configuration for a client application/service
    public class ClientConfig
    {
        // Unique identifier for the client
        public string ClientId { get; set; } = string.Empty;

        // Secret key for the client used for authentication
        public string ClientSecret { get; set; } = string.Empty;

        // The scope or permissions assigned to the client
        public string Scope { get; set; } = string.Empty;
    }
}
