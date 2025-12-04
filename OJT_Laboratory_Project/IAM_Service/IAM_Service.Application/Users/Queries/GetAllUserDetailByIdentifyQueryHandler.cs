using AutoMapper;
using IAM_Service.Application.DTOs.User;
using IAM_Service.Application.Interface.IUser;
using MediatR;

namespace IAM_Service.Application.Users.Queries
{
    /// <summary>
    /// Handler for GetUsersDetailByIdentifyNumbersQuery.
    /// </summary>
    public class GetAllUsersDetailByIdentifyNumbersQueryHandler
        : IRequestHandler<GetAllUsersDetailByIdentifyNumbersQuery, List<UserDetailDto>>
    {
        private readonly IUsersRepository _userRepository;
        private readonly IMapper _mapper;

        public GetAllUsersDetailByIdentifyNumbersQueryHandler(IUsersRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Handles the request to retrieve multiple users by IdentifyNumbers.
        /// </summary>
        public async Task<List<UserDetailDto>> Handle(
            GetAllUsersDetailByIdentifyNumbersQuery request, CancellationToken cancellationToken)
        {
            var identifyNumbers = request.IdentifyNumbers;
            var users = await _userRepository.GetAllUsersByIdentifyNumbersAsync(identifyNumbers);
            var userDetailDtos = _mapper.Map<List<UserDetailDto>>(users);

            return userDetailDtos;
        }
    }
}