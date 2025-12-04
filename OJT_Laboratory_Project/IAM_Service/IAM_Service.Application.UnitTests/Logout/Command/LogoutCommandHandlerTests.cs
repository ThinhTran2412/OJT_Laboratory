using IAM_Service.Application.Logout;
using IAM_Service.Application.Interface.IRefreshToken;
using IAM_Service.Domain.Entity;
using Moq;

namespace IAM_Service.Application.UnitTests.Logout.Command
{
    /// <summary>
    /// Unit tests for LogoutCommandHandler.
    /// </summary>
    public class LogoutCommandHandlerTests
    {
        private readonly Mock<IRefreshTokenRepository> _mockRefreshTokenRepository;
        private readonly LogoutCommandHandler _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogoutCommandHandlerTests"/> class.
        /// </summary>
        public LogoutCommandHandlerTests()
        {
            _mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
            _handler = new LogoutCommandHandler(_mockRefreshTokenRepository.Object);
        }

        // --------------------------------------------------------------------------
        // --- TEST CASE 1: SUCCESS PATH ---
        // --------------------------------------------------------------------------

        /// <summary>
        /// Handle_ValidToken_RevokesTokenSuccessfully
        /// </summary>
        [Fact]
        public async Task Handle_ValidToken_RevokesTokenSuccessfully()
        {
            // Arrange
            var refreshToken = new RefreshToken
            {
                Token = "valid-refresh-token",
                IsRevoked = false
            };

            var command = new LogoutCommand { RefreshToken = refreshToken.Token };

            _mockRefreshTokenRepository
                .Setup(r => r.GetByTokenAsync(refreshToken.Token))
                .ReturnsAsync(refreshToken);

            _mockRefreshTokenRepository
                .Setup(r => r.RevokeTokenAsync(refreshToken))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockRefreshTokenRepository.Verify(r => r.GetByTokenAsync(refreshToken.Token), Times.Once);
            _mockRefreshTokenRepository.Verify(r => r.RevokeTokenAsync(refreshToken), Times.Once);
        }

        // --------------------------------------------------------------------------
        // --- TEST CASE 2: INVALID TOKEN ---
        // --------------------------------------------------------------------------

        /// <summary>
        /// Handle_InvalidToken_ThrowsException
        /// </summary>
        [Fact]
        public async Task Handle_InvalidToken_ThrowsException()
        {
            // Arrange
            var command = new LogoutCommand { RefreshToken = "nonexistent-token" };

            _mockRefreshTokenRepository
                .Setup(r => r.GetByTokenAsync(command.RefreshToken))
                .ReturnsAsync((RefreshToken)null!);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.Equal("Invalid refresh token", ex.Message);

            _mockRefreshTokenRepository.Verify(r => r.GetByTokenAsync(command.RefreshToken), Times.Once);
            _mockRefreshTokenRepository.Verify(r => r.RevokeTokenAsync(It.IsAny<RefreshToken>()), Times.Never);
        }

        // --------------------------------------------------------------------------
        // --- TEST CASE 3: TOKEN ALREADY REVOKED ---
        // --------------------------------------------------------------------------

        /// <summary>
        /// Handle_TokenAlreadyRevoked_ThrowsException
        /// </summary>
        [Fact]
        public async Task Handle_TokenAlreadyRevoked_ThrowsException()
        {
            // Arrange
            var revokedToken = new RefreshToken
            {
                Token = "revoked-token",
                IsRevoked = true
            };

            var command = new LogoutCommand { RefreshToken = revokedToken.Token };

            _mockRefreshTokenRepository
                .Setup(r => r.GetByTokenAsync(revokedToken.Token))
                .ReturnsAsync(revokedToken);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.Equal("Token already revoked", ex.Message);

            _mockRefreshTokenRepository.Verify(r => r.GetByTokenAsync(revokedToken.Token), Times.Once);
            _mockRefreshTokenRepository.Verify(r => r.RevokeTokenAsync(It.IsAny<RefreshToken>()), Times.Never);
        }
    }
}
