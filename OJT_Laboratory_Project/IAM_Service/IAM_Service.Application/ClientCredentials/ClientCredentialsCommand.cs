using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IAM_Service.Application.ClientCredentials
{
    /// <summary>
    /// Represents the client credentials command for OAuth2 client credentials flow.
    /// This is used to capture form data sent to the API endpoint.
    /// </summary>
    public class ClientCredentialsCommand : IRequest<object>
    {
        /// <summary>
        /// The type of grant requested. Typically "client_credentials".
        /// </summary>
        [FromForm(Name = "grant_type")]
        public string Grant_Type { get; set; } = string.Empty;

        /// <summary>
        /// The unique identifier of the client.
        /// </summary>
        [FromForm(Name = "client_id")]
        public string Client_Id { get; set; } = string.Empty;

        /// <summary>
        /// The secret key of the client.
        /// </summary>
        [FromForm(Name = "client_secret")]
        public string Client_Secret { get; set; } = string.Empty;

        /// <summary>
        /// The requested scope or permissions for this client.
        /// </summary>
        [FromForm(Name = "scope")]
        public string Scope { get; set; } = string.Empty;
    }
}
