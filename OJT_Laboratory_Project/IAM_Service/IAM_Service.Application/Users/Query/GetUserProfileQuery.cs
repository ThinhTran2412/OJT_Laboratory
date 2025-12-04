using IAM_Service.Application.DTOs;
using MediatR;

namespace IAM_Service.Application.Users.Query
{
    /// <summary>
    /// Query để lấy thông tin chi tiết của một user theo UserId.
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;IAM_Service.Application.DTOs.UserProfileDTO&gt;" />
    public class GetUserProfileQuery(int userId) : IRequest<UserProfileDTO>
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public int UserId { get; set; } = userId;
    }
}
