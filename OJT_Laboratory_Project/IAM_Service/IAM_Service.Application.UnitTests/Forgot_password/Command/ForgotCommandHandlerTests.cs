using IAM_Service.Application.Common.Exceptions;
using IAM_Service.Application.forgot_password.Command;
using IAM_Service.Application.Interface.IEmailSender;
using IAM_Service.Application.Interface.IPasswordResetRepository;
using IAM_Service.Application.Interface.IUser;
using IAM_Service.Domain.Entity;
using Moq;
using Xunit;

namespace IAM_Service.Application.UnitTests.Forgot_password.Command
{
    /// <summary>
    /// Unit tests for ForgotCommandHandler
    /// </summary>
    public class ForgotCommandHandlerTests
    {
        private readonly Mock<IUsersRepository> _mockUsersRepository;
        private readonly Mock<IPasswordResetRepository> _mockPasswordResetRepository;
        private readonly Mock<IEmailSender> _mockEmailSender;

        public ForgotCommandHandlerTests()
        {
            _mockUsersRepository = new Mock<IUsersRepository>();
            _mockPasswordResetRepository = new Mock<IPasswordResetRepository>();
            _mockEmailSender = new Mock<IEmailSender>();
        }

        private ForgotCommandHandler CreateHandler()
        {
            return new ForgotCommandHandler(
                _mockUsersRepository.Object,
                _mockPasswordResetRepository.Object,
                _mockEmailSender.Object
            );
        }

        // --------------------------------------------------------------------------
        // TEST CASE 1: Success Path
        // --------------------------------------------------------------------------

        /// <summary>
        /// Should generate reset token, save to repository, and send email successfully.
        /// </summary>
        [Fact]
        public async Task Handle_ExistingEmail_ShouldGenerateToken_SaveReset_And_SendEmail()
        {
            // Arrange
            var handler = CreateHandler();
            var user = new User
            {
                UserId = 1,
                Email = "user@example.com",
                FullName = "Test User"
            };

            var command = new ForgotCommand { EmailForgot = user.Email };

            _mockUsersRepository.Setup(r => r.GetByEmailAsync(user.Email))
                .ReturnsAsync(user);

            _mockPasswordResetRepository.Setup(r => r.AddAsync(It.IsAny<PasswordReset>()))
                .Returns(Task.CompletedTask);

            _mockPasswordResetRepository.Setup(r => r.SaveChangesAsync())
                .Returns(Task.FromResult(1));

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            _mockUsersRepository.Verify(r => r.GetByEmailAsync(user.Email), Times.Once);
            _mockPasswordResetRepository.Verify(r => r.AddAsync(It.IsAny<PasswordReset>()), Times.Once);
            _mockPasswordResetRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockEmailSender.Verify(e => e.SendEmail(
                It.IsAny<string>(),
                It.IsAny<string>(),
                user.FullName,
                user.Email,
                It.IsAny<string>(),
                It.Is<string>(body => body.Contains("Reset Password"))
            ), Times.Once);
        }

        // --------------------------------------------------------------------------
        // TEST CASE 2: Invalid Email Path
        // --------------------------------------------------------------------------

        /// <summary>
        /// Should throw exception when email does not exist.
        /// </summary>
        [Fact]
        public async Task Handle_NonExistingEmail_ShouldThrowException()
        {
            // Arrange
            var handler = CreateHandler();
            var command = new ForgotCommand { EmailForgot = "notfound@example.com" };

            _mockUsersRepository.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null!);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
                handler.Handle(command, CancellationToken.None));

            Assert.Equal("Email not exist, please try again!", exception.Message);

            _mockPasswordResetRepository.Verify(r => r.AddAsync(It.IsAny<PasswordReset>()), Times.Never);
            _mockEmailSender.Verify(e => e.SendEmail(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()), Times.Never);
        }

        // --------------------------------------------------------------------------
        // TEST CASE 3: Email Sending Failure
        // --------------------------------------------------------------------------

        /// <summary>
        /// Should throw exception when email sending fails.
        /// </summary>
        [Fact]
        public async Task Handle_ExistingEmail_EmailSendingFails_ShouldThrowException()
        {
            // Arrange
            var handler = CreateHandler();
            var user = new User
            {
                UserId = 2,
                Email = "fail@example.com",
                FullName = "Fail User"
            };

            var command = new ForgotCommand { EmailForgot = user.Email };

            _mockUsersRepository.Setup(r => r.GetByEmailAsync(user.Email))
                .ReturnsAsync(user);

            _mockPasswordResetRepository.Setup(r => r.AddAsync(It.IsAny<PasswordReset>()))
                .Returns(Task.CompletedTask);

            _mockPasswordResetRepository.Setup(r => r.SaveChangesAsync())
                .Returns(Task.FromResult(1));

            _mockEmailSender.Setup(e => e.SendEmail(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
                .Throws(new Exception("SMTP server not reachable"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                handler.Handle(command, CancellationToken.None));

            Assert.Contains("SMTP server not reachable", exception.Message);

            _mockPasswordResetRepository.Verify(r => r.AddAsync(It.IsAny<PasswordReset>()), Times.Once);
            _mockEmailSender.Verify(e => e.SendEmail(
                It.IsAny<string>(),
                It.IsAny<string>(),
                user.FullName,
                user.Email,
                It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
        }
    }
}
