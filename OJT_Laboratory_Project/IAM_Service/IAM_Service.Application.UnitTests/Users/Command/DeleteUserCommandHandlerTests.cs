using IAM_Service.Application.Interface.IUser;
using IAM_Service.Application.Users.Command;
using IAM_Service.Domain.Entity;
using MediatR;
using Moq;

namespace IAM_Service.Application.Tests.Users.Command
{
    /// <summary>
    /// 
    /// </summary>
    public class DeleteUserCommandHandlerTests
    {
        /// <summary>
        /// The user repository mock
        /// </summary>
        private readonly Mock<IUsersRepository> _userRepositoryMock;
        /// <summary>
        /// The handler
        /// </summary>
        private readonly DeleteUserCommandHandler _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteUserCommandHandlerTests" /> class.
        /// </summary>
        public DeleteUserCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUsersRepository>();
            _handler = new DeleteUserCommandHandler(_userRepositoryMock.Object);
        }

        /// <summary>
        /// Handles the should delete user when user exists.
        /// </summary>
        [Fact]
        public async Task Handle_ShouldDeleteUser_WhenUserExists()
        {
            // Arrange
            var userId = 1;
            var fakeUser = new User
            {
                UserId = userId,
                FullName = "Test User",
                Email = "test@example.com"
            };

            _userRepositoryMock
                .Setup(repo => repo.GetByUserIdAsync(userId))
                .ReturnsAsync(fakeUser);

            _userRepositoryMock
                .Setup(repo => repo.DeleteAsync(fakeUser))
                .Returns(Task.CompletedTask);

            var command = new DeleteUserCommand { UserId = userId };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(Unit.Value, result);
            _userRepositoryMock.Verify(repo => repo.GetByUserIdAsync(userId), Times.Once);
            _userRepositoryMock.Verify(repo => repo.DeleteAsync(fakeUser), Times.Once);
        }

        /// <summary>
        /// Handles the should throw key not found exception when user does not exist.
        /// </summary>
        [Fact]
        public async Task Handle_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 99;

            _userRepositoryMock
                .Setup(repo => repo.GetByUserIdAsync(userId))
                .ReturnsAsync((User)null);

            var command = new DeleteUserCommand { UserId = userId };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _handler.Handle(command, CancellationToken.None));

            _userRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<User>()), Times.Never);
        }
    }
}
