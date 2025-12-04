using System;
using System.Threading;
using System.Threading.Tasks;
using IAM_Service.Application.Login;
using IAM_Service.Application.RefreshTokens.Command;
using IAM_Service.Application.Interface.IJwt;
using IAM_Service.Application.Interface.IRefreshToken;
using IAM_Service.Domain.Entity;
using Moq;
using Xunit;

namespace IAM_Service.Application.UnitTests.RefreshTokens
{
    /// <summary>
    /// Unit tests for <see cref="RefreshTokenCommandHandler"/>.
    /// Validates handling of refresh token command.
    /// </summary>
    public class RefreshTokenCommandHandlerTests
    {
        /// <summary>
        /// Tests that a valid refresh token returns a new <see cref="AuthResult"/>
        /// containing a new access token and refresh token.
        /// </summary>
        [Fact]
        public async Task Handle_ValidToken_ReturnsNewAuthResult()
        {
            // Arrange
            var mockRefreshTokenProvider = new Mock<IRefreshTokenProvider>();
            var mockRefreshTokenRepo = new Mock<IRefreshTokenRepository>();
            var mockJwtProvider = new Mock<IJwtProvider>();

            var user = new User
            {
                UserId = 1,
                FullName = "Test User",
                Email = "test@example.com"
            };

            var oldRefreshToken = new RefreshToken
            {
                Token = "old-token",
                User = user
            };

            var newRefreshToken = new RefreshToken
            {
                Token = "new-token",
                User = user
            };

            // Setup mocks
            mockRefreshTokenProvider.Setup(p => p.ValidateAsync("old-token"))
                                    .ReturnsAsync(true);
            mockRefreshTokenRepo.Setup(r => r.GetByTokenAsync("old-token"))
                                .ReturnsAsync(oldRefreshToken);
            mockJwtProvider.Setup(j => j.Generate(user))
               .ReturnsAsync("new-access-token"); // hoặc dùng Task.FromResult("new-access-token")
            mockRefreshTokenProvider.Setup(p => p.GenerateAsync(user))
                                    .ReturnsAsync(newRefreshToken);

            var handler = new RefreshTokenCommandHandler(
                mockRefreshTokenProvider.Object,
                mockRefreshTokenRepo.Object,
                mockJwtProvider.Object
            );

            var command = new RefreshTokenCommand
            {
                RefreshToken = "old-token"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("new-access-token", result.AccessToken);
            Assert.Equal("new-token", result.RefreshToken);

            // Verify interactions
            mockRefreshTokenProvider.Verify(p => p.ValidateAsync("old-token"), Times.Once);
            mockRefreshTokenRepo.Verify(r => r.GetByTokenAsync("old-token"), Times.Once);
            mockJwtProvider.Verify(j => j.Generate(user), Times.Once);
            mockRefreshTokenProvider.Verify(p => p.GenerateAsync(user), Times.Once);
            mockRefreshTokenProvider.Verify(p => p.RevokeAsync("old-token"), Times.Once);
        }

        /// <summary>
        /// Tests that an invalid refresh token throws <see cref="UnauthorizedAccessException"/>.
        /// </summary>
        [Fact]
        public async Task Handle_InvalidToken_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var mockRefreshTokenProvider = new Mock<IRefreshTokenProvider>();
            var mockRefreshTokenRepo = new Mock<IRefreshTokenRepository>();
            var mockJwtProvider = new Mock<IJwtProvider>();

            mockRefreshTokenProvider.Setup(p => p.ValidateAsync("invalid-token"))
                                    .ReturnsAsync(false);

            var handler = new RefreshTokenCommandHandler(
                mockRefreshTokenProvider.Object,
                mockRefreshTokenRepo.Object,
                mockJwtProvider.Object
            );

            var command = new RefreshTokenCommand
            {
                RefreshToken = "invalid-token"
            };

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                handler.Handle(command, CancellationToken.None)
            );

            // Verify interactions
            mockRefreshTokenProvider.Verify(p => p.ValidateAsync("invalid-token"), Times.Once);
            mockRefreshTokenRepo.Verify(r => r.GetByTokenAsync(It.IsAny<string>()), Times.Never);
            mockJwtProvider.Verify(j => j.Generate(It.IsAny<User>()), Times.Never);
        }

        /// <summary>
        /// Tests that a null token throws <see cref="ArgumentNullException"/>.
        /// </summary>
        [Fact]
        public async Task Handle_NullToken_ThrowsArgumentNullException()
        {
            // Arrange
            var mockRefreshTokenProvider = new Mock<IRefreshTokenProvider>();
            var mockRefreshTokenRepo = new Mock<IRefreshTokenRepository>();
            var mockJwtProvider = new Mock<IJwtProvider>();

            var handler = new RefreshTokenCommandHandler(
                mockRefreshTokenProvider.Object,
                mockRefreshTokenRepo.Object,
                mockJwtProvider.Object
            );

            var command = new RefreshTokenCommand
            {
                RefreshToken = null! // simulate null
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                handler.Handle(command, CancellationToken.None)
            );
        }
    }
}
