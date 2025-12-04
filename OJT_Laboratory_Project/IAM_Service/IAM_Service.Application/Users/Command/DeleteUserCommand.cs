using MediatR;

namespace IAM_Service.Application.Users.Command
{
    /// <summary>
    /// Command to permanently delete a user.
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;MediatR.Unit&gt;" />
    public class DeleteUserCommand : IRequest<Unit>
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public int UserId { get; set; }
    }
}
