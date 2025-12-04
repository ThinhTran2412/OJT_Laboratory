using MediatR;

namespace IAM_Service.Application.Roles.Command
{
    /// <summary>
    /// Command to delete a role by its ID.
    /// </summary>
    public class DeleteRoleCommand : IRequest<bool>
    {
        /// <summary>
        /// Gets or sets the role identifier.
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteRoleCommand"/> class.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        public DeleteRoleCommand(int roleId)
        {
            RoleId = roleId;
        }
    }
}
