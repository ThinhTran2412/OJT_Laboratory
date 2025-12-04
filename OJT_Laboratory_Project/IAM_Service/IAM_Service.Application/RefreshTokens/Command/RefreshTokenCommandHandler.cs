using IAM_Service.Application.Interface.IJwt;
using IAM_Service.Application.Interface.IRefreshToken;
using IAM_Service.Application.Login;
using MediatR;

namespace IAM_Service.Application.RefreshTokens.Command
{
    /// <summary>
    /// Handle RefreshTokenCommand
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;IAM_Service.Application.RefreshTokens.Command.RefreshTokenCommand, IAM_Service.Application.Login.AuthResult&gt;" />
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResult>
    {
        /// <summary>
        /// The refresh token provider
        /// </summary>
        private readonly IRefreshTokenProvider _refreshTokenProvider;
        /// <summary>
        /// The refresh token repository
        /// </summary>
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        /// <summary>
        /// The JWT provider
        /// </summary>
        private readonly IJwtProvider _jwtProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshTokenCommandHandler"/> class.
        /// </summary>
        /// <param name="refreshTokenProvider">The refresh token provider.</param>
        /// <param name="refreshTokenRepository">The refresh token repository.</param>
        /// <param name="jwtProvider">The JWT provider.</param>
        public RefreshTokenCommandHandler(
            IRefreshTokenProvider refreshTokenProvider,
            IRefreshTokenRepository refreshTokenRepository,
            IJwtProvider jwtProvider)
        {
            _refreshTokenProvider = refreshTokenProvider;
            _refreshTokenRepository = refreshTokenRepository;
            _jwtProvider = jwtProvider;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        /// <exception cref="System.UnauthorizedAccessException">Invalid or expired refresh token.</exception>
        public async Task<AuthResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            // Validate refresh token
            bool isValid = await _refreshTokenProvider.ValidateAsync(request.RefreshToken);
            if (!isValid)
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");

            var refreshTokenEntity = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

            var user = refreshTokenEntity.User;

            var accessToken = await _jwtProvider.Generate(user);


            await _refreshTokenProvider.RevokeAsync(request.RefreshToken);
            var newRefreshToken = await _refreshTokenProvider.GenerateAsync(user);

            return new AuthResult
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken.Token,
            };
        }
    }
}
