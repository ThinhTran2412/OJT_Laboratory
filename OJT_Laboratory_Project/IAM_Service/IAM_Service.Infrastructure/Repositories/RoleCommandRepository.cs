using IAM_Service.Application.Interface.IRole;
using IAM_Service.Domain.Entity;
using IAM_Service.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IAM_Service.Infrastructure.Repositories
{
    /// <summary>
    /// implement method from IRoleCommandRepository
    /// </summary>
    /// <seealso cref="IAM_Service.Application.Interface.IRole.IRoleCommandRepository" />
    public class RoleCommandRepository : IRoleCommandRepository
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly AppDbContext _context;
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleCommandRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public RoleCommandRepository(AppDbContext context) => _context = context;

        /// <summary>
        /// Check if a role with the given code already exists.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<bool> ExistsByCodeAsync(string code, CancellationToken ct = default)
            => _context.Roles.AnyAsync(r => r.Code == code, ct);

        /// <summary>
        /// Add a new role to the database context.
        /// </summary>
        /// <param name="role"></param>
        /// <param name="ct"></param>
        public async Task AddAsync(Role role, CancellationToken ct = default)
            => await _context.Roles.AddAsync(role, ct);

        /// <summary>
        /// Update an existing role.
        /// </summary>
        /// <param name="role">The role with updated information.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>True if the role was updated, false if it was not found.</returns>
        public async Task<bool> UpdateAsync(Role role, CancellationToken ct = default)
        {
            var existingRole = await _context.Roles
                .Include(r => r.RolePrivileges)
                .FirstOrDefaultAsync(r => r.RoleId == role.RoleId, ct);

            if (existingRole == null)
                return false;

            // Update scalar properties
            _context.Entry(existingRole).CurrentValues.SetValues(role);

            // Remove old RolePrivileges not in the updated role
            foreach (var existingRolePrivilege in existingRole.RolePrivileges.ToList())
            {
                if (!role.RolePrivileges.Any(rp => rp.PrivilegeId == existingRolePrivilege.PrivilegeId))
                {
                    _context.RolePrivileges.Remove(existingRolePrivilege);
                }
            }

            // Add new RolePrivileges
            foreach (var rolePrivilege in role.RolePrivileges)
            {
                if (!existingRole.RolePrivileges.Any(rp => rp.PrivilegeId == rolePrivilege.PrivilegeId))
                {
                    existingRole.RolePrivileges.Add(new Domain.Entity.RolePrivilege
                    {
                        RoleId = existingRole.RoleId,
                        PrivilegeId = rolePrivilege.PrivilegeId
                    });
                }
            }

            await _context.SaveChangesAsync(ct);
            return true;
        }

        /// <summary>
        /// Get a role by its ID with tracking.
        /// </summary>
        public async Task<Role?> GetByIdWithTrackingAsync(int roleId, CancellationToken ct = default)
        {
            return await _context.Roles
                .Include(r => r.RolePrivileges)
                .FirstOrDefaultAsync(r => r.RoleId == roleId, ct);
        }

        /// <summary>
        /// Check if a role with the given ID exists.
        /// </summary>
        public Task<bool> ExistsByIdAsync(int roleId, CancellationToken ct = default)
        {
            return _context.Roles.AnyAsync(r => r.RoleId == roleId, ct);
        }

        /// <summary>
        /// Persist all pending changes.
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task SaveChangesAsync(CancellationToken ct = default)
            => _context.SaveChangesAsync(ct);

        /// <summary>
        /// Delete a role from the database.
        /// </summary>
        /// <param name="roleId">The ID of the role to delete.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>True if the role was deleted, false if it was not found.</returns>
        public async Task<bool> DeleteAsync(int roleId, CancellationToken ct = default)
        {
            // Find the role including its RolePrivilege relationships
            var role = await _context.Roles
                .Include(r => r.RolePrivileges)
                .FirstOrDefaultAsync(r => r.RoleId == roleId, ct);

            if (role == null)
                return false;

            // Remove all RolePrivilege relationships first to avoid reference constraint
            _context.RolePrivileges.RemoveRange(role.RolePrivileges);
            
            // Then remove the role itself
            _context.Roles.Remove(role);
            
            // Save changes to the database
            await _context.SaveChangesAsync(ct);
            
            return true;
        }
    }
}
