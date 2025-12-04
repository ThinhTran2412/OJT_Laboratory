using MediatR;
using IAM_Service.Application.DTOs.Pagination;
using IAM_Service.Application.DTOs.Roles;

namespace IAM_Service.Application.Roles.Query
{
    /// <summary>
    /// Query model bound from query string for listing roles.
    /// Supports search, paging, and sorting.
    /// </summary>
    public class GetRolesQuery : IRequest<PagedResponse<RoleListItemDto>>
    {
        /// <summary>
        /// Keyword to search by Role Name, Code, Description, or Privilege Name (case-insensitive).
        /// </summary>
        public string? Search { get; set; }
        
        /// <summary>Page number (1-based). Default is 1.</summary>
        public int Page { get; set; } = 1;
        
        /// <summary>Page size. Default is 10.</summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Sort column: id | name | code | description. Default is id when not provided.
        /// </summary>
        public string? SortBy { get; set; }

        /// <summary>
        /// Sort direction. Default is false (ascending).
        /// </summary>
        public bool SortDesc { get; set; } = false;

        /// <summary>
        /// List of privilege IDs to filter roles by. 
        /// Only roles that have ALL specified privileges will be returned (AND logic).
        /// </summary>
        public List<int>? PrivilegeIds { get; set; }
    }
}


