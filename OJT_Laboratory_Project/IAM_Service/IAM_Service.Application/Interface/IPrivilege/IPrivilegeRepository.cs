using IAM_Service.Domain.Entity;

namespace IAM_Service.Application.Interface.IPrivilege
{
    /// <summary>
    /// Defines the contract for data access operations related to <see cref="Privilege" /> entities.
    /// </summary>
    public interface IPrivilegeRepository
    {
        /// <summary>
        /// Asynchronously retrieves all privileges in the system.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>
        /// A list of all <see cref="Privilege" /> entities.
        /// </returns>
        Task<List<Privilege>> GetAllPrivilegesAsync(CancellationToken cancellationToken);
        /// <summary>
        /// Gets the privileges by ids asynchronous.
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <param name="ct">The ct.</param>
        /// <returns></returns>
        Task<List<Privilege>> GetPrivilegesByIdsAsync(List<int> ids, CancellationToken ct = default);
        /// <summary>
        /// Gets the by name asynchronous.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="ct">The ct.</param>
        /// <returns></returns>
        Task<Privilege?> GetByNameAsync(string name, CancellationToken ct = default);
        /// <summary>
        /// Gets the privilege names by role identifier asynchronous.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <returns></returns>
        Task<List<string>> GetPrivilegeNamesByRoleIdAsync(int? roleId);
        /// <summary>
        /// Gets the privilege names by user identifier asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        Task<List<string>> GetPrivilegeNamesByUserIdAsync(int? userId);
    }
}
