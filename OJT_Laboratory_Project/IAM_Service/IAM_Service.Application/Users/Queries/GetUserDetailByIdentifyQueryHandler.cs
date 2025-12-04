using AutoMapper;
using IAM_Service.Application.DTOs.User;
using IAM_Service.Application.Interface.IUser;
using MediatR;

namespace IAM_Service.Application.Users.Queries
{
    /// <summary>
    /// Handler for GetUserDetailByIdentifyQueryHandler
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;IAM_Service.Application.Users.Queries.GetUserDetailByIdentifyQuery, IAM_Service.Application.DTOs.UserDetailDto&gt;" />
    public class GetUserDetailByIdentifyQueryHandler
        : IRequestHandler<GetUserDetailByIdentifyQuery, UserDetailDto?>
    {
        /// <summary>
        /// The user repository
        /// </summary>
        private readonly IUsersRepository _userRepository;
        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetUserDetailByIdentifyQueryHandler"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="mapper">The mapper.</param>
        public GetUserDetailByIdentifyQueryHandler(IUsersRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
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
        public async Task<UserDetailDto?> Handle(
            GetUserDetailByIdentifyQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUsersByIdentifyNumbersAsync(request.IdentifyNumber);

            if (user == null)
            {
                return null;
            }

            var userDetailDto = _mapper.Map<UserDetailDto>(user);

            return userDetailDto;
        }
    }
}