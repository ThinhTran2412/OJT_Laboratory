using IAM_Service.Application.DTOs.User;
using MediatR;

namespace IAM_Service.Application.Users.Queries
{
    /// <summary>
    /// Query to GetUserDetailByIdentify
    /// </summary>
    public record GetAllUsersDetailByIdentifyNumbersQuery(List<string> IdentifyNumbers) : IRequest<List<UserDetailDto>>;
}
