using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using IAM_Service.Application.Common.Exceptions;
using IAM_Service.Application.Interface.IPasswordHasher;
using IAM_Service.Application.Interface.IPasswordResetRepository;
using IAM_Service.Application.Interface.IUser;
using IAM_Service.Application.ResetPassword;
using IAM_Service.Domain.Entity;
using Moq;
using Xunit;

namespace IAM_Service.Application.UnitTests.ResetPassword
{
    /// <summary>
    /// Unit tests for <see cref="ResetPasswordCommand"/> and <see cref="ResetPasswordCommandHandler"/>.
    /// </summary>
    public class ResetPasswordCommandTests
    {
        #region CommandHandler Tests

        /// <summary>
        /// Tests that handler successfully resets the password for a valid token and existing user.
        /// </summary>
        [Fact]
        public async Task Handle_Should_ResetPassword_When_TokenIsValid()
        {
            // Arrange
            var mockUserRepo = new Mock<IUsersRepository>();
            var mockPasswordHasher = new Mock<IPasswordHasher>();
            var mockResetRepo = new Mock<IPasswordResetRepository>();

            var command = new ResetPasswordCommand
            {
                Token = "valid-token",
                NewPassword = "NewPassword@123"
            };

            var resetRecord = new PasswordReset
            {
                Token = "valid-token",
                UserId = 1,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false
            };

            var user = new User { UserId = 1, Password = "old-pass" };

            mockResetRepo.Setup(r => r.GetByTokenAsync(command.Token))
                         .ReturnsAsync(resetRecord);
            mockUserRepo.Setup(r => r.GetByIdAsync(resetRecord.UserId))
                        .ReturnsAsync(user);
            mockPasswordHasher.Setup(h => h.Hash(command.NewPassword))
                              .Returns("hashed-new-password");
            mockUserRepo.Setup(r => r.UpdateAsync(user))
                        .Returns(Task.CompletedTask);
            mockResetRepo.Setup(r => r.MarkUsedAsync(resetRecord))
                         .Returns(Task.CompletedTask);

            var handler = new ResetPasswordCommandHandler(
                mockUserRepo.Object,
                mockPasswordHasher.Object,
                mockResetRepo.Object
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(MediatR.Unit.Value, result);
            Assert.Equal("hashed-new-password", user.Password);
            mockResetRepo.Verify(r => r.GetByTokenAsync(command.Token), Times.Once);
            mockUserRepo.Verify(r => r.GetByIdAsync(user.UserId), Times.Once);
            mockPasswordHasher.Verify(h => h.Hash(command.NewPassword), Times.Once);
            mockUserRepo.Verify(r => r.UpdateAsync(user), Times.Once);
            mockResetRepo.Verify(r => r.MarkUsedAsync(resetRecord), Times.Once);
        }

        /// <summary>
        /// Tests that handler throws <see cref="InvalidOrExpiredTokenException"/> when token is invalid, expired, or already used.
        /// </summary>
        [Fact]
        public async Task Handle_Should_ThrowInvalidOrExpiredTokenException_When_TokenInvalidOrExpired()
        {
            // Arrange
            var mockUserRepo = new Mock<IUsersRepository>();
            var mockPasswordHasher = new Mock<IPasswordHasher>();
            var mockResetRepo = new Mock<IPasswordResetRepository>();

            var command = new ResetPasswordCommand
            {
                Token = "invalid-token",
                NewPassword = "NewPassword@123"
            };

            mockResetRepo.Setup(r => r.GetByTokenAsync(command.Token))
                         .ReturnsAsync((PasswordReset?)null);

            var handler = new ResetPasswordCommandHandler(
                mockUserRepo.Object,
                mockPasswordHasher.Object,
                mockResetRepo.Object
            );

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOrExpiredTokenException>(() =>
                handler.Handle(command, CancellationToken.None)
            );

            mockResetRepo.Verify(r => r.GetByTokenAsync(command.Token), Times.Once);
            mockUserRepo.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        /// <summary>
        /// Tests that handler throws <see cref="NotFoundException"/> when user does not exist for a valid token.
        /// </summary>
        [Fact]
        public async Task Handle_Should_ThrowNotFoundException_When_UserNotFound()
        {
            // Arrange
            var mockUserRepo = new Mock<IUsersRepository>();
            var mockPasswordHasher = new Mock<IPasswordHasher>();
            var mockResetRepo = new Mock<IPasswordResetRepository>();

            var resetRecord = new PasswordReset
            {
                Token = "valid-token",
                UserId = 1,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false
            };

            var command = new ResetPasswordCommand
            {
                Token = "valid-token",
                NewPassword = "NewPassword@123"
            };

            mockResetRepo.Setup(r => r.GetByTokenAsync(command.Token))
                         .ReturnsAsync(resetRecord);
            mockUserRepo.Setup(r => r.GetByIdAsync(resetRecord.UserId))
                        .ReturnsAsync((User?)null);

            var handler = new ResetPasswordCommandHandler(
                mockUserRepo.Object,
                mockPasswordHasher.Object,
                mockResetRepo.Object
            );

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                handler.Handle(command, CancellationToken.None)
            );

            mockResetRepo.Verify(r => r.GetByTokenAsync(command.Token), Times.Once);
            mockUserRepo.Verify(r => r.GetByIdAsync(resetRecord.UserId), Times.Once);
        }

        #endregion
    }
}
