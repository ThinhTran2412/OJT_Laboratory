using IAM_Service.Application.DTOs.User;
using MediatR;

namespace IAM_Service.Application.Users.Queries
{
    /// <summary>
    /// create record for GetUserDetailByIdentifyQuery
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;IAM_Service.Application.DTOs.UserDetailDto&gt;" />
    /// <seealso cref="MediatR.IBaseRequest" />
    /// <seealso cref="System.IEquatable&lt;IAM_Service.Application.Users.Queries.GetUserDetailByIdentifyQuery&gt;" />
    public record GetUserDetailByIdentifyQuery(string IdentifyNumber) : IRequest<UserDetailDto?>;
}
