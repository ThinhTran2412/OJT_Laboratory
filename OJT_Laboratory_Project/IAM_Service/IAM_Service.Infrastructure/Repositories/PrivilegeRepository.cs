using IAM_Service.Application.Interface.IPrivilege;
using IAM_Service.Domain.Entity;
using IAM_Service.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IAM_Service.Infrastructure.Repositories
{
    /// <summary>
    /// Implement method from IPrivilegeRepository
    /// </summary>
    /// <seealso cref="IAM_Service.Application.Interface.IPrivilege.IPrivilegeRepository" />
    public class PrivilegeRepository : IPrivilegeRepository
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly AppDbContext _context;
        /// <summary>
        /// Initializes a new instance of the <see cref="PrivilegeRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public PrivilegeRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Asynchronously retrieves all privileges in the system.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>
        /// A list of all <see cref="T:IAM_Service.Domain.Entity.Privilege" /> entities.
        /// </returns>
        public async Task<List<Privilege>> GetAllPrivilegesAsync(CancellationToken cancellationToken)
        {
            return await _context.Privileges.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets the by name asynchronous.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="ct">The ct.</param>
        /// <returns></returns>
        public async Task<Privilege?> GetByNameAsync(string name, CancellationToken ct = default)
        {
            return await _context.Privileges
                .FirstOrDefaultAsync(p => p.Name == name, ct);
        }

        /// <summary>
        /// Gets the privilege names by role identifier asynchronous.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <returns></returns>
        public async Task<List<string>> GetPrivilegeNamesByRoleIdAsync(int? roleId)
        {
            var privilegeNames = await _context.RolePrivileges
                .Where(rp => rp.RoleId == roleId)
                .Include(rp => rp.Privilege)
                .Select(rp => rp.Privilege.Name)
                .ToListAsync();

            return privilegeNames;
        }

        /// <summary>
        /// Gets the privileges by ids asynchronous.
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <param name="ct">The ct.</param>
        /// <returns></returns>
        public async Task<List<Privilege>> GetPrivilegesByIdsAsync(List<int> ids, CancellationToken ct = default)
        {
            return await _context.Privileges
                .Where(p => ids.Contains(p.PrivilegeId))
                .ToListAsync(ct);
        }
        /// <summary>
        /// Gets the privilege names by user identifier asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public Task<List<string>> GetPrivilegeNamesByUserIdAsync(int? userId)
        {
            var privilegeNames = _context.UserPrivileges
                .Where(up => up.UserId == userId)
                .Include(up => up.Privilege)
                .Select(up => up.Privilege.Name)
                .ToList();

            return Task.FromResult(privilegeNames);
        }


    }
}
