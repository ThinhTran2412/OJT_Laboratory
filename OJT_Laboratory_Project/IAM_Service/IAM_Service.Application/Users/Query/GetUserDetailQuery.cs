using IAM_Service.Application.DTOs.User;
using MediatR;

namespace IAM_Service.Application.Users.Query
{
    /// <summary>
    /// Create attribute for class GetUserDetailQuery
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;UserDetailDto&gt;" />
    public class GetUserDetailQuery : IRequest<UserDetailDto?>
    {
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string Email { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="GetUserDetailQuery"/> class.
        /// </summary>
        /// <param name="email">The email.</param>
        public GetUserDetailQuery(string email) => Email = email;
    }
}
