using IAM_Service.Application.Interface.IClient;
using IAM_Service.Infrastructure.Configuration;
using IAM_Service.Application.DTOs;

namespace IAM_Service.Infrastructure.Authentication
{
    // Repository for managing client information for authentication purposes
    public class ClientRepository : IClientRepository
    {
        /// <summary>
        /// Finds a client based on clientId and clientSecret
        /// </summary>
        /// <param name="clientId">The client ID</param>
        /// <param name="clientSecret">The client secret</param>
        /// <returns>
        /// Returns a ServiceIdentity if a valid client is found, 
        /// or null if not found or credentials do not match
        /// </returns>
        public async Task<ServiceIdentity?> FindClientByCredentialsAsync(string clientId, string clientSecret)
        {
            // Await a completed task to satisfy the async signature, no real async operation here
            await Task.CompletedTask;

            // Retrieve the client from ClientStore based on clientId and clientSecret
            var clientConfig = ClientStore.Clients.FirstOrDefault(c =>
                c.ClientId == clientId && c.ClientSecret == clientSecret);

            // Return null if client not found or clientSecret does not match
            if (clientConfig == null || clientConfig.ClientSecret != clientSecret)
            {
                return null;
            }

            // Return client information as a ServiceIdentity
            return new ServiceIdentity
            {
                ClientId = clientConfig.ClientId,
                Scope = clientConfig.Scope
            };
        }
    }
}
