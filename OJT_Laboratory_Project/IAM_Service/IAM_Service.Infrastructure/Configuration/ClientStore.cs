namespace IAM_Service.Infrastructure.Configuration
{
    // Static store to hold valid clients (acting as a replacement for a database
    // or IdentityServer configuration)
    public static class ClientStore
    {
        // List of valid clients
        // In a real application, this would come from a secure database or configuration
        public static readonly List<ClientConfig> Clients = new List<ClientConfig>
        {
            new ClientConfig
            {
                ClientId = "laboratory_service_client", // Unique identifier for the client
                ClientSecret = "your_highly_secure_secret_lab", // SECRET! Replace with a strong password
                Scope = "iam_user_read" // The access scope/permissions of this client
            }
        };

        /// <summary>
        /// Retrieves the client configuration by clientId
        /// </summary>
        /// <param name="clientId">The client ID to search for</param>
        /// <returns>
        /// Returns the ClientConfig if found, otherwise null
        /// </returns>
        public static ClientConfig? GetClientConfig(string clientId)
        {
            // Find the first client with the matching clientId
            return Clients.FirstOrDefault(c => c.ClientId == clientId);
        }
    }
}
