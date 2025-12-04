using IAM_Service.Application.DTOs.Roles;
using MediatR;

namespace IAM_Service.Application.Roles.Command
{

    /// <summary>
    /// create record for CreateRoleCommand
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;IAM_Service.Application.DTOs.Roles.RoleDto&gt;" />
    /// <seealso cref="MediatR.IBaseRequest" />
    /// <seealso cref="System.IEquatable&lt;IAM_Service.Application.Roles.Command.CreateRoleCommand&gt;" />
    public record CreateRoleCommand(RoleCreateDto Dto) : IRequest<RoleDto>;

}
