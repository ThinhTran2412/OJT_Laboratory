using IAM_Service.Application.DTOs.Roles;
using MediatR;

namespace IAM_Service.Application.Roles.Command
{
    /// <summary>
    /// Command to update an existing role.
    /// </summary>
    public class UpdateRoleCommand : IRequest<RoleDto>
    {
        /// <summary>
        /// Gets or sets the role ID.
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Gets or sets the role data transfer object.
        /// </summary>
        public UpdateRoleDto Dto { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateRoleCommand"/> class.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="dto">The role data transfer object.</param>
        public UpdateRoleCommand(int roleId, UpdateRoleDto dto)
        {
            RoleId = roleId;
            Dto = dto;
        }
    }
}
