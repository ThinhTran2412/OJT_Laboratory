using Laboratory_Service.Domain.Entity;

namespace Laboratory_Service.Application.Interface
{
    /// <summary>
    /// Interface for IAM Service communication
    /// </summary>
    public interface IIAMService
    {
        /// <summary>
        /// Get user information by identification number
        /// </summary>
        /// <param name="identifyNumber">User's identification number</param>
        /// <returns>User information or null if not found</returns>
        Task<UserInfo?> GetUserByIdentifyNumberAsync(string identifyNumber);

        /// <summary>
        /// Get user information by user ID
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>User information or null if not found</returns>
        Task<UserInfo?> GetUserByIdAsync(int userId);

        /// <summary>
        /// Validate user permissions for patient operations
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="operation">Operation type (CREATE, READ, UPDATE, DELETE)</param>
        /// <returns>True if user has permission, false otherwise</returns>
        Task<bool> ValidateUserPermissionAsync(int userId, string operation);

        /// <summary>
        /// Get user role information
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>User role information</returns>
        Task<string?> GetUserRoleAsync(int userId);
    }
}
