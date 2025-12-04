using IAM_Service.Application.DTOs.User;
using MediatR;

namespace IAM_Service.Application.Users.Queries
{
    /// <summary>
    /// Create attribute for class GetUserByIdQuery
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;IAM_Service.Application.DTOs.UserDetailDto&gt;" />
    public class GetUserByIdQuery : IRequest<UserDetailDto>
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }
    }
}