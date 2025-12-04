using IAM_Service.Domain.Entity;

namespace IAM_Service.Application.Interface.IUser
{
    /// <summary>
    /// Create method for User Repository
    /// </summary>
    public interface IUsersRepository
    {
        /// <summary>
        /// Gets the by email asynchronous.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        Task<User> GetByEmailAsync(string email);
        /// <summary>
        /// Gets the by password asynchronous.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        Task<User> GetByPasswordAsync(string password);
        /// <summary>
        /// Creates the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        Task CreateUser(User user);
        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        Task UpdateAsync(User user);
        /// <summary>
        /// Saves the changes asynchronous.
        /// </summary>
        /// <returns></returns>
        Task<int> SaveChangesAsync();


        /// <summary>
        /// Gets the user asynchronous.
        /// </summary>
        /// <returns></returns>
        Task<List<User>> GetUserAsync();

        /// <summary>
        /// Gets the user by identifier asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Task<User> GetUserByIdAsync(int id);
        /// <summary>
        /// Gets the user by identify number asynchronous.
        /// </summary>
        /// <param name="identifyNumbers">The identify numbers.</param>
        /// <returns></returns>
        Task<IEnumerable<User>> GetAllUsersByIdentifyNumbersAsync(List<string> identifyNumbers);
        /// <summary>
        /// Checks the user exists by identify number asynchronous.
        /// </summary>
        /// <param name="identifyNumber">The identify number.</param>
        /// <returns></returns>
        Task<bool> CheckUserExistsByIdentifyNumberAsync(string identifyNumber);
        /// <summary>
        /// Gets the users by identify numbers asynchronous.
        /// </summary>
        /// <param name="identifyNumbers">The identify numbers.</param>
        /// <returns></returns>
        Task<User?> GetUsersByIdentifyNumbersAsync(string identifyNumbers);

        /// Gets the user by identifier.
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <returns>
        /// The user if found; otherwise, null.
        /// </returns>
        Task<User?> GetByIdAsync(int id);

        /// <summary>
        /// Updates the user asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        Task UpdateUserAsync(User entity);
        /// <summary>
        /// Permanently deletes a user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        Task DeleteAsync(User? user);
        /// <summary>
        /// Gets the by user identifier asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        Task<User?> GetByUserIdAsync(int userId);
        /// <summary>
        /// Retrieves a user by their UserId.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<IEnumerable<int>> GetUserPrivilegesAsync(int userId);

        /// <summary>
        /// Adds the user privileges asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="privilegeIds">The privilege ids.</param>
        /// <returns></returns>
        Task AddUserPrivilegesAsync(int userId, List<int> privilegeIds);
        /// <summary>
        /// Removes the user privileges asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="privilegeIds">The privilege ids.</param>
        /// <returns></returns>
        Task RemoveUserPrivilegesAsync(int userId, List<int> privilegeIds);

        /// <summary>
        /// Gets the original privileges asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        Task<List<int>> GetOriginalPrivilegesAsync(int userId);
    }
}
