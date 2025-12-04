using IAM_Service.Application.DTOs;

namespace IAM_Service.Application.Interface.IClient
{
    /// <summary>
    /// Repository interface for managing client authentication.
    /// </summary>
    public interface IClientRepository
    {
        /// <summary>
        /// Finds a client based on the provided clientId and clientSecret.
        /// </summary>
        /// <param name="clientId">The unique identifier of the client.</param>
        /// <param name="clientSecret">The secret key of the client.</param>
        /// <returns>
        /// Returns a <see cref="ServiceIdentity"/> if a valid client is found; otherwise, null.
        /// </returns>
        Task<ServiceIdentity?> FindClientByCredentialsAsync(string clientId, string clientSecret);
    }
}
