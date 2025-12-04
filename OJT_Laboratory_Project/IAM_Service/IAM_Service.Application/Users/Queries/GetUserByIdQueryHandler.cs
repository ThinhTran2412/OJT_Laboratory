using IAM_Service.Application.DTOs.User;
using IAM_Service.Application.Interface.IUser;
using MediatR;

namespace IAM_Service.Application.Users.Queries
{
    /// <summary>
    /// Handler for GetUserByIdQuery
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;IAM_Service.Application.Users.Queries.GetUserByIdQuery, IAM_Service.Application.DTOs.UserDetailDto&gt;" />
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDetailDto>
    {
        /// <summary>
        /// The user repository
        /// </summary>
        private readonly IUsersRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetUserByIdQueryHandler"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        public GetUserByIdQueryHandler(IUsersRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<UserDetailDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(request.Id);
            if (user == null)
                return null;

            return new UserDetailDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IdentifyNumber = user.IdentifyNumber,
                RoleId = user.RoleId
            };
        }
    }
}