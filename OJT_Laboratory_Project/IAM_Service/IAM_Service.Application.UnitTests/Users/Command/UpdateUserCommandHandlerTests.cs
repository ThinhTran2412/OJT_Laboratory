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
    public class UpdateUserCommandHandlerTests
    {
        /// <summary>
        /// The user repo mock
        /// </summary>
        private readonly Mock<IUsersRepository> _userRepoMock;
        /// <summary>
        /// The handler
        /// </summary>
        private readonly UpdateUserCommandHandler _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateUserCommandHandlerTests"/> class.
        /// </summary>
        public UpdateUserCommandHandlerTests()
        {
            _userRepoMock = new Mock<IUsersRepository>();

            _handler = new UpdateUserCommandHandler(
                _userRepoMock.Object
            );
        }

        /// <summary>
        /// Handles the should update fields when provided.
        /// </summary>
        [Fact]
        public async Task Handle_Should_Update_Fields_When_Provided()
        {
            var user = new User { UserId = 1, FullName = "Old Name" };

            var command = new UpdateUserCommand { UserId = 1, FullName = "New Name" };

            _userRepoMock.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.UpdateUserAsync(user)).Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.Equal("New Name", user.FullName);
            _userRepoMock.Verify(r => r.UpdateUserAsync(user), Times.Once);
            _userRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        /// <summary>
        /// Handles the should not update empty fields.
        /// </summary>
        [Fact]
        public async Task Handle_Should_Not_Update_Empty_Fields()
        {
            var user = new User { UserId = 1, FullName = "Old Name" };

            var command = new UpdateUserCommand { UserId = 1, FullName = "" };

            _userRepoMock.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync(user);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.Equal("Old Name", user.FullName);
        }

        /// <summary>
        /// Handles the should throw when user not found.
        /// </summary>
        [Fact]
        public async Task Handle_Should_Throw_When_User_Not_Found()
        {
            var command = new UpdateUserCommand { UserId = 999 };

            _userRepoMock.Setup(r => r.GetByUserIdAsync(999)).ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }

        /// <summary>
        /// Handles the should add privileges when action type is add.
        /// </summary>
        [Fact]
        public async Task Handle_Should_Add_Privileges_When_ActionType_Is_Add()
        {
            var user = new User { UserId = 1 };

            var command = new UpdateUserCommand
            {
                UserId = 1,
                ActionType = "add",
                PrivilegeIds = new List<int> { 1, 2, 3 }
            };

            _userRepoMock.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.GetUserPrivilegesAsync(1)).ReturnsAsync(new List<int> { 1 });
            _userRepoMock.Setup(r => r.AddUserPrivilegesAsync(1, It.IsAny<List<int>>()))
                         .Returns(Task.CompletedTask);

            await _handler.Handle(command, CancellationToken.None);

            _userRepoMock.Verify(r => r.AddUserPrivilegesAsync(1, It.Is<List<int>>(l => l.SequenceEqual(new[] { 2, 3 }))), Times.Once);
            _userRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        /// <summary>
        /// Handles the should reset privileges when action type is reset.
        /// </summary>
        [Fact]
        public async Task Handle_Should_Reset_Privileges_When_ActionType_Is_Reset()
        {
            var user = new User { UserId = 1 };

            var command = new UpdateUserCommand { UserId = 1, ActionType = "reset" };

            _userRepoMock.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.GetOriginalPrivilegesAsync(1))
                         .ReturnsAsync(new List<int> { 1, 2 });
            _userRepoMock.Setup(r => r.GetUserPrivilegesAsync(1))
                         .ReturnsAsync(new List<int> { 1, 2, 3 });

            _userRepoMock.Setup(r => r.RemoveUserPrivilegesAsync(1, It.IsAny<List<int>>()))
                         .Returns(Task.CompletedTask);

            await _handler.Handle(command, CancellationToken.None);

            _userRepoMock.Verify(r => r.RemoveUserPrivilegesAsync(1, It.Is<List<int>>(l => l.SequenceEqual(new[] { 3 }))), Times.Once);
            _userRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        /// <summary>
        /// Handles the should synchronize privileges when action type is null.
        /// </summary>
        [Fact]
        public async Task Handle_Should_Sync_Privileges_When_ActionType_Is_Null()
        {
            var user = new User { UserId = 1 };

            var command = new UpdateUserCommand
            {
                UserId = 1,
                PrivilegeIds = new List<int> { 1, 3 }
            };

            _userRepoMock.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.GetUserPrivilegesAsync(1))
                         .ReturnsAsync(new List<int> { 1, 2 });

            await _handler.Handle(command, CancellationToken.None);

            // newPrivileges = {3}
            _userRepoMock.Verify(r => r.AddUserPrivilegesAsync(1, It.Is<List<int>>(l => l.SequenceEqual(new[] { 3 }))), Times.Once);

            // removedPrivileges = {2}
            _userRepoMock.Verify(r => r.RemoveUserPrivilegesAsync(1, It.Is<List<int>>(l => l.SequenceEqual(new[] { 2 }))), Times.Once);

            _userRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        /// <summary>
        /// Handles the should not call update when no fields changed.
        /// </summary>
        [Fact]
        public async Task Handle_Should_Not_Call_Update_When_No_Fields_Changed()
        {
            var user = new User { UserId = 1, FullName = "Name" };

            var command = new UpdateUserCommand { UserId = 1 };

            _userRepoMock.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync(user);

            await _handler.Handle(command, CancellationToken.None);

            _userRepoMock.Verify(r => r.UpdateUserAsync(It.IsAny<User>()), Times.Never);
        }

        /// <summary>
        /// Handles the should not modify privileges when privilege ids null.
        /// </summary>
        [Fact]
        public async Task Handle_Should_Not_Modify_Privileges_When_PrivilegeIds_Null()
        {
            var user = new User { UserId = 1 };

            var command = new UpdateUserCommand { UserId = 1 };

            _userRepoMock.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync(user);

            await _handler.Handle(command, CancellationToken.None);

            _userRepoMock.Verify(r => r.AddUserPrivilegesAsync(It.IsAny<int>(), It.IsAny<List<int>>()), Times.Never);
            _userRepoMock.Verify(r => r.RemoveUserPrivilegesAsync(It.IsAny<int>(), It.IsAny<List<int>>()), Times.Never);
        }
    }
}
