using MediatR;

namespace IAM_Service.Application.forgot_password.Command
{
    /// <summary>
    /// Create ForgotCommand
    /// </summary>
    /// <seealso cref="MediatR.IRequest" />
    public class ForgotCommand : IRequest
    {
        /// <summary>
        /// Gets or sets the email forgot.
        /// </summary>
        /// <value>
        /// The email forgot.
        /// </value>
        public string EmailForgot { get; set; }
    }
}
