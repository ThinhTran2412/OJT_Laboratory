using AutoMapper;
using IAM_Service.Application.DTOs;
using IAM_Service.Application.Interface.IUser;
using MediatR;

namespace IAM_Service.Application.Users.Query
{
    /// <summary>
    /// Handler cho query lấy thông tin chi tiết user.
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;IAM_Service.Application.Users.Query.GetUserProfileQuery, IAM_Service.Application.DTOs.UserProfileDTO&gt;" />
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfileDTO>
    {
        /// <summary>
        /// The users repository
        /// </summary>
        private readonly IUsersRepository _usersRepository;
        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetUserProfileQueryHandler"/> class.
        /// </summary>
        /// <param name="usersRepository">The users repository.</param>
        /// <param name="mapper">The mapper.</param>
        public GetUserProfileQueryHandler(IUsersRepository usersRepository, IMapper mapper)
        {
            _usersRepository = usersRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">User with ID {request.UserId} not found.</exception>
        public async Task<UserProfileDTO> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            var user = await _usersRepository.GetByUserIdAsync(request.UserId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {request.UserId} not found.");

            // Tự động map sang UserDTO
            return _mapper.Map<UserProfileDTO>(user);
        }
    }
}
