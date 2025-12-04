namespace IAM_Service.Application.DTOs
{
    /// <summary>
    /// Represents the identity of a service/client after successful authentication.
    /// </summary>
    public class ServiceIdentity
    {
        /// <summary>
        /// The unique identifier of the client.
        /// </summary>
        public string ClientId { get; set; } = string.Empty;

        /// <summary>
        /// The access scope or permissions granted to the client.
        /// </summary>
        public string Scope { get; set; } = string.Empty;
    }
}
