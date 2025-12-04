using IAM_Service.Application.Interface.IJwt;
using IAM_Service.Application.Interface.IRefreshToken;
using IAM_Service.Domain.Entity;

namespace IAM_Service.Infrastructure.Authentication
{
    /// <summary>
    /// Implement methods from IRefreshTokenProvider to create new token
    /// </summary>
    /// <seealso cref="IAM_Service.Application.Interface.IJwt.IRefreshTokenProvider" />
    public sealed class RefreshTokenProvider : IRefreshTokenProvider
    {
        /// <summary>
        /// The repository
        /// </summary>
        private readonly IRefreshTokenRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshTokenProvider"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public RefreshTokenProvider(IRefreshTokenRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Generates the asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public async Task<RefreshToken> GenerateAsync(User user)
        {
            var refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                UserId = user.UserId,
                IsRevoked = false
            };

            await _repository.AddAsync(refreshToken);
            return refreshToken;
        }

        /// <summary>
        /// Validates the asynchronous.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public async Task<bool> ValidateAsync(string token)
        {
            var rt = await _repository.GetByTokenAsync(token);
            return rt != null && !rt.IsRevoked && rt.ExpiryDate > DateTime.UtcNow;
        }

        /// <summary>
        /// Revokes the asynchronous.
        /// </summary>
        /// <param name="token">The token.</param>
        public async Task RevokeAsync(string token)
        {
            await _repository.RevokeAsync(token);
        }
    }
}
