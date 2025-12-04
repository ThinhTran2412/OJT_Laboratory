using Laboratory_Service.Application.DTOs.User;

namespace Laboratory_Service.Application.Interface
{
    /// <summary>
    /// Defines methods for interacting with IAM Service (user-related operations)
    /// </summary>
    public interface IIamUserService
    {
        /// <summary>
        /// Gets the user by identify number asynchronously.
        /// </summary>
        /// <param name="identifyNumber">The identify number.</param>
        /// <returns>The user data or null if not found.</returns>
        Task<UserDataDTO?> GetUserByIdentifyNumberAsync(string identifyNumber);

        /// <summary>
        /// Checks if a user exists asynchronously.
        /// </summary>
        /// <param name="identifyNumber">The identify number.</param>
        /// <returns>True if the user exists; otherwise, false.</returns>
        Task<bool> CheckUserExistsAsync(string identifyNumber);

        /// <summary>
        /// Gets multiple users by their identify numbers asynchronously.
        /// </summary>
        /// <param name="identifyNumbers">List of identify numbers.</param>
        /// <returns>List of user data.</returns>
        Task<List<UserDataDTO>> GetUsersByIdentifyNumbersAsync(List<string> identifyNumbers);

        /// <summary>
        /// Creates a new user in IAM Service asynchronously.
        /// </summary>
        /// <param name="userDetails">The user details to create.</param>
        /// <returns>True if the user was created successfully; otherwise, false.</returns>
        Task<bool> CreateUserAsync(UserDataDTO userDetails);
    }
}
