using IAM_Service.Domain.Entity;

namespace IAM_Service.Application.Interface.IRole
{

    /// <summary>
    /// Command repository for write operations on Role.
    /// </summary>
    public interface IRoleCommandRepository
    {
        /// <summary>Check if a role with the given code already exists.</summary>
        Task<bool> ExistsByCodeAsync(string code, CancellationToken ct = default);

        /// <summary>Check if a role with the given ID exists.</summary>
        Task<bool> ExistsByIdAsync(int roleId, CancellationToken ct = default);

        /// <summary>Get a role by its ID with tracking.</summary>
        Task<Role?> GetByIdWithTrackingAsync(int roleId, CancellationToken ct = default);

        /// <summary>Add a new role to the database context.</summary>
        Task AddAsync(Role role, CancellationToken ct = default);

        /// <summary>Update an existing role.</summary>
        /// <param name="role">The role with updated information.</param>
        /// <returns>True if the role was updated, false if it was not found.</returns>
        Task<bool> UpdateAsync(Role role, CancellationToken ct = default);

        /// <summary>Persist all pending changes.</summary>
        Task SaveChangesAsync(CancellationToken ct = default);

        /// <summary>Delete a role from the database.</summary>
        /// <param name="roleId">The ID of the role to delete.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>True if the role was deleted, false if it was not found.</returns>
        Task<bool> DeleteAsync(int roleId, CancellationToken ct = default);
    }
}
