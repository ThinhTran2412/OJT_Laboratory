using IAM_Service.Application.Interface.IRefreshToken;
using MediatR;

namespace IAM_Service.Application.Logout
{
    /// <summary>
    /// Create Handler
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;IAM_Service.Application.Logout.LogoutCommand&gt;" />
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
    {
        /// <summary>
        /// The refresh token repository
        /// </summary>
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogoutCommandHandler"/> class.
        /// </summary>
        /// <param name="refreshTokenRepository">The refresh token repository.</param>
        public LogoutCommandHandler(IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <exception cref="System.Exception">
        /// Invalid refresh token
        /// or
        /// Token already revoked
        /// </exception>
        public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var token = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);
            if (token == null)
                throw new Exception("Invalid refresh token");

            if (token.IsRevoked)
                throw new Exception("Token already revoked");

            await _refreshTokenRepository.RevokeTokenAsync(token);
        }
    }
}
