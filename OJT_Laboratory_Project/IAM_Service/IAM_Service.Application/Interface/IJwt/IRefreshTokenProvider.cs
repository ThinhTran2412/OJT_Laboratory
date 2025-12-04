using IAM_Service.Domain.Entity;

namespace IAM_Service.Application.Interface.IJwt
{
    /// <summary>
    /// create methods for interface RefreshTokenProvider
    /// </summary>
    public interface IRefreshTokenProvider
    {
        /// <summary>
        /// Generates the asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        Task<RefreshToken> GenerateAsync(User user);
        /// <summary>
        /// Validates the asynchronous.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        Task<bool> ValidateAsync(string token);
        /// <summary>
        /// Revokes the asynchronous.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        Task RevokeAsync(string token);
    }
}
