using IAM_Service.Application.DTOs.Pagination;
using IAM_Service.Domain.Entity;

namespace IAM_Service.Application.Interface.IRole
{
    public interface IRoleRepository
    {
        /// <summary>
        /// Query roles with search, paging and sorting. Returns a paged result for handlers.
        /// </summary>
        Task<PagedResult<Role>> GetRolesAsync(
            string? search,
            int page,
            int pageSize,
            string? sortBy,
            bool sortDesc,
            List<int>? privilegeIds,
            CancellationToken cancellationToken);

        /// <summary>
        /// Get a role by its ID.
        /// </summary>
        Task<Role?> GetByIdAsync(int roleId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a role by its code.
        /// </summary>
        Task<Role?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    }
}


