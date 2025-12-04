using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using IAM_Service.Application.DTOs.Privileges;
using IAM_Service.Application.Interface.IPrivilege;
using MediatR;

namespace IAM_Service.Application.Privileges.Query
{
    /// <summary>
    /// Handles <see cref="GetPrivilegesQuery"/> by querying the repository
    /// and mapping domain entities to DTOs for API consumption.
    /// This handler retrieves all privileges in the system for use in dropdowns and filters.
    /// </summary>
    public class GetPrivilegesQueryHandler : IRequestHandler<GetPrivilegesQuery, List<PrivilegeDto>>
    {
        private readonly IPrivilegeRepository _privilegeRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetPrivilegesQueryHandler"/> class.
        /// </summary>
        /// <param name="privilegeRepository">The repository for accessing privilege data.</param>
        public GetPrivilegesQueryHandler(IPrivilegeRepository privilegeRepository)
        {
            _privilegeRepository = privilegeRepository;
        }

        /// <summary>
        /// Handles the <see cref="GetPrivilegesQuery"/> to retrieve all privileges.
        /// </summary>
        /// <param name="request">The query request (no parameters needed).</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of <see cref="PrivilegeDto"/> containing all privileges.</returns>
        public async Task<List<PrivilegeDto>> Handle(GetPrivilegesQuery request, CancellationToken cancellationToken)
        {
            // 1. Query all privileges from the repository
            // The repository handles the database interaction with optimized read-only queries
            var privileges = await _privilegeRepository.GetAllPrivilegesAsync(cancellationToken);

            // 2. Map domain entities (Privilege) to DTOs (PrivilegeDto) for API consumption
            // This step transforms the database-centric entities into a format suitable for the API client
            var dtoList = privileges.Select(p => new PrivilegeDto
            {
                PrivilegeId = p.PrivilegeId,
                Name = p.Name,
                Description = p.Description
            }).ToList();

            // 3. Return the mapped DTOs
            return dtoList;
        }
    }
}
