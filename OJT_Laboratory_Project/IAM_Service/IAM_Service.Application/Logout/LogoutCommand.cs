using MediatR;

namespace IAM_Service.Application.Logout
{
    /// <summary>
    /// Create Logout command
    /// </summary>
    /// <seealso cref="MediatR.IRequest" />
    public class LogoutCommand : IRequest
    {
        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        /// <value>
        /// The refresh token.
        /// </value>
        public string RefreshToken { get; set; } = string.Empty;
    }
}
