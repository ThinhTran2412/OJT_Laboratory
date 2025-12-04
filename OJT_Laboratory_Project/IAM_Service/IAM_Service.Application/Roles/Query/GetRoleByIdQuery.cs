using IAM_Service.Application.DTOs.Roles;
using MediatR;

namespace IAM_Service.Application.Roles.Query
{
    /// <summary>
    /// Query to get a role by its ID.
    /// </summary>
    public class GetRoleByIdQuery : IRequest<RoleDto>
    {
        /// <summary>
        /// Gets or sets the role identifier.
        /// </summary>
        public int RoleId { get; set; }
    }
}
