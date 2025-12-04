using AutoMapper;
using IAM_Service.Application.DTOs.User;
using IAM_Service.Application.Interface.IPrivilege;
using IAM_Service.Application.Interface.IRole;
using IAM_Service.Application.Interface.IUser;
using MediatR;

namespace IAM_Service.Application.Users.Query
{
    /// <summary>
    /// Handle get user detail information
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;IAM_Service.Application.Users.Query.GetUserDetailQuery, UserDetailDto&gt;" />
    public class GetUserDetailQueryHandler : IRequestHandler<GetUserDetailQuery, UserDetailDto?>
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
        /// The role repository
        /// </summary>
        private readonly IRoleRepository _roleRepository;
        /// <summary>
        /// The privilege repository
        /// </summary>
        private readonly IPrivilegeRepository _privilegeRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetUserDetailQueryHandler"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="roleRepository">The role repository.</param>
        /// <param name="privilegeRepository">The privilege repository.</param>
        public GetUserDetailQueryHandler(
            IUsersRepository userRepository,
            IMapper mapper,
            IRoleRepository roleRepository,
            IPrivilegeRepository privilegeRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _roleRepository = roleRepository;
            _privilegeRepository = privilegeRepository;
        }
        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<UserDetailDto?> Handle(GetUserDetailQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
                return null; 

            var dto = _mapper.Map<UserDetailDto>(user);

            if (user.RoleId.HasValue && user.RoleId.Value > 0)
            {
                var role = await _roleRepository.GetByIdAsync(user.RoleId.Value, cancellationToken);
                dto.RoleId = user.RoleId.Value;
                dto.RoleName = role?.Name;
                dto.RoleCode = role?.Code;
            }

            // Enrich Privileges - Get privileges from both RoleId and UserId
            var rolePrivileges = await _privilegeRepository.GetPrivilegeNamesByRoleIdAsync(user.RoleId) ?? new System.Collections.Generic.List<string>();
            var userPrivileges = await _privilegeRepository.GetPrivilegeNamesByUserIdAsync(user.UserId) ?? new System.Collections.Generic.List<string>();

            // Combine both lists and remove duplicates
            dto.Privileges = rolePrivileges.Union(userPrivileges).ToList();

            return dto;
        }
    }
}