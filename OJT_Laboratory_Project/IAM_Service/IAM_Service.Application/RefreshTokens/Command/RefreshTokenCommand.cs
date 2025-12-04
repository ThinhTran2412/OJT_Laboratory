using IAM_Service.Application.Login;
using MediatR;

namespace IAM_Service.Application.RefreshTokens.Command
{
    /// <summary>
    /// create attribute for class RefreshTokenCommand
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;IAM_Service.Application.Login.AuthResult&gt;" />
    public class RefreshTokenCommand : IRequest<AuthResult>
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
