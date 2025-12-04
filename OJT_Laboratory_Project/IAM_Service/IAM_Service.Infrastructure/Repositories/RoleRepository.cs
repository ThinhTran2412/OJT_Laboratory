using IAM_Service.Application.DTOs.Pagination;
using IAM_Service.Application.Interface.IRole;
using IAM_Service.Domain.Entity;
using IAM_Service.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IAM_Service.Infrastructure.Repositories
{
    /// <summary>
    /// Data access for roles.
    /// Responsibilities:
    /// - Compose an EF Core IQueryable with eager loading of privileges.
    /// - Apply keyword search over role fields and privilege names (case-insensitive).
    /// - Apply sorting on a whitelisted set of columns (id|name|code|description).
    /// - Count BEFORE paging, then apply Skip/Take for efficient pagination.
    /// - Use AsNoTracking for read-only performance.
    /// </summary>
    /// <seealso cref="IAM_Service.Application.Interface.IRole.IRoleRepository" />
    public class RoleRepository : IRoleRepository
    {
        /// <summary>
        /// The database context
        /// </summary>
        private readonly AppDbContext _dbContext;
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleRepository" /> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public RoleRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Query roles with search, paging and sorting. Returns a paged result for handlers.
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortBy"></param>
        /// <param name="sortDesc"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<PagedResult<Role>> GetRolesAsync(
            string? search,
            int page,
            int pageSize,
            string? sortBy,
            bool sortDesc,
            List<int>? privilegeIds,
            CancellationToken cancellationToken)
        {
            // Base query with eager loading for privileges. No tracking for read performance.
            IQueryable<Role> query = _dbContext.Roles
            .Include(r => r.RolePrivileges)
            .ThenInclude(rp => rp.Privilege)
            .AsNoTracking();

            // Apply privilege filter first (AND logic - role must have ALL specified privileges)
            if (privilegeIds != null && privilegeIds.Any())
            {
                // For each privilege ID, ensure the role has that privilege
                foreach (int privilegeId in privilegeIds)
                {
                    query = query.Where(r => r.RolePrivileges.Any(rp => rp.PrivilegeId == privilegeId));
                }
            }

            // Apply search filter if provided (after privilege filtering)
            if (!string.IsNullOrWhiteSpace(search))
            {
                string s = search.Trim().ToLower();
                query = query.Where(r => r.Name.ToLower().Contains(s)
                    || r.Code.ToLower().Contains(s)
                    || r.Description.ToLower().Contains(s)
                    || r.RolePrivileges.Any(rp => rp.Privilege.Name.ToLower().Contains(s))
                );
            }
            // Apply sorting by requested column (default to id)
            if (string.IsNullOrEmpty(sortBy))
            {
                sortBy = "id";
            }
            string sort = sortBy.ToLower();

            switch (sort)
            {
                case "id":
                    query = sortDesc ? query.OrderByDescending(r => r.RoleId) : query.OrderBy(r => r.RoleId);
                    break;
                case "code":
                    query = sortDesc ? query.OrderByDescending(r => r.Code) : query.OrderBy(r => r.Code);
                    break;
                case "description":
                    query = sortDesc ? query.OrderByDescending(r => r.Description) : query.OrderBy(r => r.Description);
                    break;
                default:
                    query = sortDesc ? query.OrderByDescending(r => r.Name) : query.OrderBy(r => r.Name);
                    break;
            }

            // Count BEFORE paging to get total records
            int total = await query.CountAsync(cancellationToken);

            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            // Apply paging at the database level
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<Role>
            {
                Items = items,
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Get a role by its ID.
        /// </summary>
        public async Task<Role?> GetByIdAsync(int roleId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Roles
                .Include(r => r.RolePrivileges)
                .ThenInclude(rp => rp.Privilege)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.RoleId == roleId, cancellationToken);
        }

        /// <summary>
        /// Get a role by its code.
        /// </summary>
        public async Task<Role?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Roles
                .Include(r => r.RolePrivileges)
                .ThenInclude(rp => rp.Privilege)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Code == code, cancellationToken);
        }
    }
}


