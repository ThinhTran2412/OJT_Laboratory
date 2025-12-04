using AutoMapper;
using IAM_Service.Application.DTOs.Roles;
using IAM_Service.Application.Interface.IRole;
using MediatR;
using Microsoft.Extensions.Logging;

namespace IAM_Service.Application.Roles.Query
{
    /// <summary>
    /// Handles the retrieval of a single role by its ID.
    /// </summary>
    public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, RoleDto>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetRoleByIdQueryHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetRoleByIdQueryHandler"/> class.
        /// </summary>
        public GetRoleByIdQueryHandler(
            IRoleRepository roleRepository,
            IMapper mapper,
            ILogger<GetRoleByIdQueryHandler> logger)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the get role by ID request.
        /// </summary>
        public async Task<RoleDto> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving role with ID {RoleId}", request.RoleId);

            var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
            
            if (role == null)
            {
                _logger.LogWarning("Role with ID {RoleId} not found", request.RoleId);
                return null;
            }

            var roleDto = _mapper.Map<RoleDto>(role);
            roleDto.Privileges = role.RolePrivileges
                .Select(rp => rp.Privilege?.Name)
                .Where(name => name != null)
                .ToList();

            _logger.LogInformation("Successfully retrieved role with ID {RoleId}", request.RoleId);
            return roleDto;
        }
    }
}
