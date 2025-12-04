using MediatR;

/// <summary>
/// Command representing a user login request.
/// </summary>
namespace IAM_Service.Application.Login
{
    /// <summary>
    /// Command for user login, containing email and password.
    /// </summary>
    public class LoginCommand : IRequest<AuthResult>
    {
        /// <summary>
        /// The email of the user attempting to log in.
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// The password of the user attempting to log in.
        /// </summary>
        public string Password { get; set; }
    }
}
