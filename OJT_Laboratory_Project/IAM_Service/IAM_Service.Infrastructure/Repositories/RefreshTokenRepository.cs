using IAM_Service.Application.Interface.IRefreshToken;
using IAM_Service.Domain.Entity;
using IAM_Service.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IAM_Service.Infrastructure.Repositories
{
    /// <summary>
    /// Implement method from IRefreshTokenRepository
    /// </summary>
    /// <seealso cref="IAM_Service.Application.Interface.IRefreshToken.IRefreshTokenRepository" />
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshTokenRepository" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets the by token asynchronous.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }

        /// <summary>
        /// Adds the asynchronous.
        /// </summary>
        /// <param name="refreshToken">The refresh token.</param>
        public async Task AddAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Revokes the asynchronous.
        /// </summary>
        /// <param name="token">The token.</param>
        public async Task RevokeAsync(string token)
        {
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token);
            if (refreshToken != null)
            {
                refreshToken.IsRevoked = true;
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Determines whether [is valid asynchronous] [the specified token].
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>
        ///   <c>true</c> if [is valid asynchronous] [the specified token]; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> IsValidAsync(string token)
        {
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token);
            return refreshToken != null && !refreshToken.IsRevoked && refreshToken.ExpiryDate > DateTime.UtcNow;
        }

        /// <summary>
        /// Revokes the token asynchronous.
        /// </summary>
        /// <param name="token">The token.</param>
        public async Task RevokeTokenAsync(RefreshToken token)
        {
            token.IsRevoked = true;
            _context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync();
        }
    }
}
