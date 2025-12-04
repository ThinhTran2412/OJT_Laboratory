using Xunit;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using IAM_Service.Application.Roles.Command;
using IAM_Service.Application.Interface.IRole;

namespace IAM_Service.Application.UnitTests.Roles.Command
{
    public class DeleteRoleCommandHandlerTests
    {
        private readonly Mock<IRoleCommandRepository> _roleCommandRepoMock;
        private readonly Mock<ILogger<DeleteRoleCommandHandler>> _loggerMock;
        private readonly DeleteRoleCommandHandler _handler;

        public DeleteRoleCommandHandlerTests()
        {
            _roleCommandRepoMock = new Mock<IRoleCommandRepository>();
            _loggerMock = new Mock<ILogger<DeleteRoleCommandHandler>>();

            _handler = new DeleteRoleCommandHandler(
                _roleCommandRepoMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Handle_Should_ReturnTrue_When_Delete_Successful()
        {
            // ARRANGE
            var command = new DeleteRoleCommand(1);
            _roleCommandRepoMock
                .Setup(r => r.DeleteAsync(command.RoleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // ACT
            var result = await _handler.Handle(command, CancellationToken.None);

            // ASSERT
            Assert.True(result);

            _roleCommandRepoMock.Verify(r =>
                r.DeleteAsync(command.RoleId, It.IsAny<CancellationToken>()), Times.Once);

            _loggerMock.Verify(l =>
                l.Log(LogLevel.Information,
                      It.IsAny<EventId>(),
                      It.IsAny<It.IsAnyType>(),
                      It.IsAny<Exception>(),
                      (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task Handle_Should_ReturnFalse_When_Role_Not_Found()
        {
            // ARRANGE
            var command = new DeleteRoleCommand(999);
            _roleCommandRepoMock
                .Setup(r => r.DeleteAsync(command.RoleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // ACT
            var result = await _handler.Handle(command, CancellationToken.None);

            // ASSERT
            Assert.False(result);

            _loggerMock.Verify(l =>
                l.Log(LogLevel.Warning,
                      It.IsAny<EventId>(),
                      It.IsAny<It.IsAnyType>(),
                      It.IsAny<Exception>(),
                      (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task Handle_Should_LogError_And_Throw_When_ExceptionOccurs()
        {
            // ARRANGE
            var command = new DeleteRoleCommand(5);

            _roleCommandRepoMock
                .Setup(r => r.DeleteAsync(command.RoleId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database connection failed"));

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.Equal("Database connection failed", ex.Message);

            _loggerMock.Verify(l =>
                l.Log(LogLevel.Error,
                      It.IsAny<EventId>(),
                      It.IsAny<It.IsAnyType>(),
                      It.IsAny<Exception>(),
                      (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }
    }
}
