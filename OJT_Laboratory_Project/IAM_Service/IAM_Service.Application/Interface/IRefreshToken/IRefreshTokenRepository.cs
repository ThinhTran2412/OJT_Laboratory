using IAM_Service.Domain.Entity;

namespace IAM_Service.Application.Interface.IRefreshToken
{
    /// <summary>
    ///  create methods for interface RefreshTokenRepository
    /// </summary>
    public interface IRefreshTokenRepository
    {
        /// <summary>
        /// Gets the by token asynchronous.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        Task<RefreshToken?> GetByTokenAsync(string token);
        /// <summary>
        /// Adds the asynchronous.
        /// </summary>
        /// <param name="refreshToken">The refresh token.</param>
        /// <returns></returns>
        Task AddAsync(RefreshToken refreshToken);
        /// <summary>
        /// Revokes the asynchronous.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        Task RevokeAsync(string token);
        /// <summary>
        /// Determines whether [is valid asynchronous] [the specified token].
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        Task<bool> IsValidAsync(string token);

        Task RevokeTokenAsync(RefreshToken token);
    }
}
