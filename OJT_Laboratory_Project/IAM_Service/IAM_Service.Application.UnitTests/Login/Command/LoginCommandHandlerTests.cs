using IAM_Service.Application.Common.Exceptions;
using IAM_Service.Application.Common.Security;
using IAM_Service.Application.Interface.IJwt;
using IAM_Service.Application.Interface.IPasswordHasher;
using IAM_Service.Application.Interface.IUser;
using IAM_Service.Application.Login;
using IAM_Service.Domain.Entity;
using Microsoft.Extensions.Options;
using Moq;

namespace IAM_Service.Application.UnitTests.Login.Command
{
    public class LoginCommandHandlerTests
    {
        private readonly Mock<IUsersRepository> _mockUserRepository;
        private readonly Mock<IJwtProvider> _mockJwtProvider;
        private readonly Mock<IPasswordHasher> _mockPasswordHasher;
        private readonly Mock<IRefreshTokenProvider> _mockRefreshTokenProvider;

        private readonly string _expectedAccessToken = "fake-access-token";
        private readonly string _expectedRefreshToken = "fake-refresh-token";
        private readonly LoginCommand _testCommand = new LoginCommand { Email = "test@example.com", Password = "validpassword" };
        private readonly string _userPassword = "hashedpassword";

        public LoginCommandHandlerTests()
        {
            _mockUserRepository = new Mock<IUsersRepository>();
            _mockJwtProvider = new Mock<IJwtProvider>();
            _mockPasswordHasher = new Mock<IPasswordHasher>();
            _mockRefreshTokenProvider = new Mock<IRefreshTokenProvider>();

            // Setup async methods correctly
            _mockJwtProvider.Setup(p => p.Generate(It.IsAny<User>()))
                            .ReturnsAsync(_expectedAccessToken);

            _mockRefreshTokenProvider.Setup(p => p.GenerateAsync(It.IsAny<User>()))
                                     .ReturnsAsync(new RefreshToken { Token = _expectedRefreshToken, ExpiryDate = DateTime.UtcNow.AddDays(7) });

            _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>()))
                               .Returns(Task.CompletedTask);

            _mockUserRepository.Setup(r => r.SaveChangesAsync())
                               .ReturnsAsync(1);
        }

        private LoginCommandHandler CreateHandler(bool isLockoutEnabled, int maxAttempts = 3, int lockoutMins = 5)
        {
            var lockoutOptions = Options.Create(new LockoutOptions
            {
                Enabled = isLockoutEnabled,
                MaxFailedAccessAttempts = maxAttempts,
                LockoutMinutes = lockoutMins
            });

            return new LoginCommandHandler(
                _mockUserRepository.Object,
                _mockJwtProvider.Object,
                _mockPasswordHasher.Object,
                lockoutOptions,
                _mockRefreshTokenProvider.Object);
        }

        // ------------------------ SUCCESS CASES ------------------------
        [Fact]
        public async Task Handle_ValidCredentials_LockoutEnabled_ResetsAttemptsAndReturnsAuthResult()
        {
            var handler = CreateHandler(true);
            var user = new User { Email = _testCommand.Email, Password = _userPassword, FailedLoginAttempts = 1, LockoutEnd = DateTime.UtcNow.AddMinutes(-5) };

            _mockUserRepository.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                               .ReturnsAsync(user);
            _mockPasswordHasher.Setup(h => h.Verify(_testCommand.Password, _userPassword)).Returns(true);

            var result = await handler.Handle(_testCommand, CancellationToken.None);

            Assert.Equal(_expectedAccessToken, result.AccessToken);
            Assert.Equal(_expectedRefreshToken, result.RefreshToken);
            Assert.Equal(0, user.FailedLoginAttempts);
            Assert.Null(user.LockoutEnd);

            _mockUserRepository.Verify(r => r.UpdateAsync(user), Times.Once);
            _mockJwtProvider.Verify(p => p.Generate(user), Times.Once);
            _mockRefreshTokenProvider.Verify(p => p.GenerateAsync(user), Times.Once);
        }

        [Fact]
        public async Task Handle_ValidCredentials_LockoutDisabled_NoRepositoryUpdateAndReturnsAuthResult()
        {
            var handler = CreateHandler(false);
            var user = new User { Email = _testCommand.Email, Password = _userPassword, FailedLoginAttempts = 5 };

            _mockUserRepository.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                               .ReturnsAsync(user);
            _mockPasswordHasher.Setup(h => h.Verify(_testCommand.Password, _userPassword)).Returns(true);

            var result = await handler.Handle(_testCommand, CancellationToken.None);

            Assert.Equal(_expectedAccessToken, result.AccessToken);
            Assert.Equal(_expectedRefreshToken, result.RefreshToken);

            _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        // ------------------------ ACCOUNT LOCKED ------------------------
        [Fact]
        public async Task Handle_AccountIsCurrentlyLocked_ThrowsAccountLockedException()
        {
            var handler = CreateHandler(true);
            var futureLockout = DateTime.UtcNow.AddMinutes(10);
            var user = new User { Email = _testCommand.Email, Password = _userPassword, LockoutEnd = futureLockout };

            _mockUserRepository.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                               .ReturnsAsync(user);

            var ex = await Assert.ThrowsAsync<AccountLockedException>(() =>
                handler.Handle(_testCommand, CancellationToken.None));

            Assert.Contains("Account locked until", ex.Message);
            Assert.True(ex.TimeRemainingInSeconds > 0);

            _mockPasswordHasher.Verify(h => h.Verify(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        // ------------------------ INVALID CREDENTIALS ------------------------
        [Fact]
        public async Task Handle_UserNotFound_ThrowsInvalidCredentialsException()
        {
            var handler = CreateHandler(true, maxAttempts: 5);
            _mockUserRepository.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                               .ReturnsAsync((User)null!);

            var ex = await Assert.ThrowsAsync<InvalidCredentialsException>(() =>
                handler.Handle(_testCommand, CancellationToken.None));

            Assert.Equal("Invalid credentials.", ex.Message);
            Assert.Equal(5, ex.AttemptsRemaining);
            _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Handle_InvalidPassword_LockoutDisabled_ThrowsInvalidCredentialsException_NoUpdate()
        {
            var handler = CreateHandler(false, maxAttempts: 5);
            var user = new User { Email = _testCommand.Email, Password = _userPassword, FailedLoginAttempts = 1 };

            _mockUserRepository.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                               .ReturnsAsync(user);
            _mockPasswordHasher.Setup(h => h.Verify(_testCommand.Password, _userPassword)).Returns(false);

            var ex = await Assert.ThrowsAsync<InvalidCredentialsException>(() =>
                handler.Handle(_testCommand, CancellationToken.None));

            Assert.Equal(0, ex.AttemptsRemaining);
            Assert.Equal(1, user.FailedLoginAttempts);
            _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Handle_InvalidPassword_LockoutEnabled_IncrementsAttempts_ThrowsInvalidCredentialsException()
        {
            var handler = CreateHandler(true, maxAttempts: 5);
            var user = new User { Email = _testCommand.Email, Password = _userPassword, FailedLoginAttempts = 1 };

            _mockUserRepository.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                               .ReturnsAsync(user);
            _mockPasswordHasher.Setup(h => h.Verify(_testCommand.Password, _userPassword)).Returns(false);

            var ex = await Assert.ThrowsAsync<InvalidCredentialsException>(() =>
                handler.Handle(_testCommand, CancellationToken.None));

            Assert.Equal(2, user.FailedLoginAttempts);
            Assert.Equal(3, ex.AttemptsRemaining); // 5 - 2
            _mockUserRepository.Verify(r => r.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidPassword_MaxAttemptsReached_LocksAccount_ThrowsAccountLockedException()
        {
            var handler = CreateHandler(true, maxAttempts: 3, lockoutMins: 10);
            var user = new User { Email = _testCommand.Email, Password = _userPassword, FailedLoginAttempts = 2 };

            _mockUserRepository.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                               .ReturnsAsync(user);
            _mockPasswordHasher.Setup(h => h.Verify(_testCommand.Password, _userPassword)).Returns(false);

            var ex = await Assert.ThrowsAsync<AccountLockedException>(() =>
                handler.Handle(_testCommand, CancellationToken.None));

            Assert.True(user.LockoutEnd.HasValue);
            Assert.True(user.LockoutEnd.Value > DateTime.UtcNow);
            Assert.Equal(0, user.FailedLoginAttempts);
            Assert.Equal(10 * 60, ex.TimeRemainingInSeconds);

            _mockUserRepository.Verify(r => r.UpdateAsync(user), Times.Once);
        }
    }
}
