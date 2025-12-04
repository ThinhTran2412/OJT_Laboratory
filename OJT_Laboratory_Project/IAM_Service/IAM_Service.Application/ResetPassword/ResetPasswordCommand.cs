using MediatR;

namespace IAM_Service.Application.ResetPassword
{
    /// <summary>
    /// Create ResetPasswordCommand
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;MediatR.Unit&gt;" />
    public class ResetPasswordCommand : IRequest<Unit>
    {
        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        /// <value>
        /// The token.
        /// </value>
        public string Token { get; set; }
        /// <summary>
        /// Creates new password.
        /// </summary>
        /// <value>
        /// The new password.
        /// </value>
        public string NewPassword { get; set; }
    }
}
