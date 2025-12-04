using AutoMapper;
using IAM_Service.Application.DTOs.Pagination;
using IAM_Service.Application.DTOs.Privileges;
using IAM_Service.Application.DTOs.Roles;
using IAM_Service.Application.Interface.IRole;
using MediatR;

namespace IAM_Service.Application.Roles.Query
{
    /// <summary>
    /// Handles <see cref="GetRolesQuery" />.
    /// Responsibilities:
    /// 1) Fetch a paged result of Role entities from repository based on query params
    /// (search keyword, page, pageSize, sortBy, sortDesc).
    /// 2) Project domain entities -&gt; transport-friendly DTOs used by API/UI.
    /// 3) Wrap the list with paging metadata and a friendly message when empty.
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;IAM_Service.Application.Roles.Query.GetRolesQuery, IAM_Service.Application.DTOs.Pagination.PagedResponse&lt;IAM_Service.Application.DTOs.Roles.RoleListItemDto&gt;&gt;" />
    public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, PagedResponse<RoleListItemDto>>
    {
        /// <summary>
        /// The role repository
        /// </summary>
        private readonly IRoleRepository _roleRepository;
        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;
        /// <summary>
        /// Initializes a new instance of the <see cref="GetRolesQueryHandler"/> class.
        /// </summary>
        /// <param name="roleRepository">The role repository.</param>
        /// <param name="mapper">The mapper.</param>
        public GetRolesQueryHandler(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Main execution of the query.
        /// </summary>
        /// <param name="request">Incoming parameters from query string. Example:
        /// - Search: "adm"
        /// - Page/PageSize: 1/10
        /// - SortBy/SortDesc: "name"/false (ascending by name)</param>
        /// <param name="cancellationToken">Propagated cancellation token from ASP.NET Core pipeline.</param>
        /// <returns>
        /// PagedResponse of RoleListItemDto ready to be serialized to JSON.
        /// </returns>
        public async Task<PagedResponse<RoleListItemDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            // 1) Query repository: this runs at database level (EF Core -> SQL), not in-memory.
            //    It applies search (Role + Privilege), sorting, and only then paging for efficiency.
            var result = await _roleRepository.GetRolesAsync(
                request.Search,
                request.Page,
                request.PageSize,
                request.SortBy,
                request.SortDesc,
                request.PrivilegeIds,
                cancellationToken);

            // 2) Map entities -> DTOs using AutoMapper. We avoid exposing domain navigation directly to API.
            var dtoItems = new List<RoleListItemDto>();

            foreach (var r in result.Items)
            {
                List<PrivilegeDto> privileges;

                privileges = _mapper.Map<List<PrivilegeDto>>(
                    r.RolePrivileges.Select(rp => rp.Privilege).ToList()
                );

                dtoItems.Add(new RoleListItemDto
                {
                    RoleId = r.RoleId,
                    Name = r.Name,
                    Code = r.Code,
                    Description = r.Description,
                    Privileges = privileges
                });
            }

            // 3) Return a page wrapper with metadata (Total/Page/PageSize) and a friendly message when empty.
            return new PagedResponse<RoleListItemDto>
            {
                Items = dtoItems,
                Total = result.Total,
                Page = result.Page,
                PageSize = result.PageSize,
                Message = dtoItems.Count == 0 ? "No Data" : null
            };
        }
    }
}


