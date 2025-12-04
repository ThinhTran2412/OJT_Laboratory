using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using IAM_Service.Application.DTOs.Roles;
using IAM_Service.Application.Interface.IPrivilege;
using IAM_Service.Application.Interface.IRole;
using IAM_Service.Application.Roles.Command;
using IAM_Service.Domain.Entity;
using Moq;
using Xunit;

namespace IAM_Service.Application.UnitTests.Roles.Command
{
    /// <summary>
    /// Unit tests for <see cref="CreateRoleCommandHandler"/>.
    /// </summary>
    public class CreateRoleCommandHandlerTests
    {
        private readonly Mock<IRoleCommandRepository> _roleRepoMock;
        private readonly Mock<IPrivilegeRepository> _privRepoMock;
        private readonly CreateRoleCommandHandler _handler;
        private readonly Mock<IPrivilegeNormalizationService> _privilegeNormalizationServiceMock; 

        /// <summary>
        /// Initializes a new instance of <see cref="CreateRoleCommandHandlerTests"/>.
        /// Sets up mocks for repositories and the handler instance.
        /// </summary>
        public CreateRoleCommandHandlerTests()
        {
            _roleRepoMock = new Mock<IRoleCommandRepository>();
            _privRepoMock = new Mock<IPrivilegeRepository>();
            _handler = new CreateRoleCommandHandler(_roleRepoMock.Object, _privRepoMock.Object,_privilegeNormalizationServiceMock.Object);
        }

        /// <summary>
        /// Test that an <see cref="InvalidOperationException"/> is thrown
        /// when the role code already exists in the database.
        /// </summary>
        [Fact]
        public async Task Handle_ShouldThrow_WhenRoleCodeExists()
        {
            var dto = new RoleCreateDto { Name = "Role", Code = "CODE" };
            var command = new CreateRoleCommand(dto);

            _roleRepoMock.Setup(r => r.ExistsByCodeAsync("CODE", It.IsAny<CancellationToken>()))
                         .ReturnsAsync(true);

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<InvalidOperationException>()
                     .WithMessage("Role with code 'CODE' already exists.");
        }

        /// <summary>
        /// Test that when no privilege IDs are provided, the role
        /// is assigned the default "READ_ONLY" privilege.
        /// </summary>
        [Fact]
        public async Task Handle_ShouldAssignReadOnly_WhenNoPrivilegeIds()
        {
            var dto = new RoleCreateDto { Name = "Role", Code = "CODE", PrivilegeIds = new List<int>() };
            var command = new CreateRoleCommand(dto);

            _roleRepoMock.Setup(r => r.ExistsByCodeAsync("CODE", It.IsAny<CancellationToken>()))
                         .ReturnsAsync(false);
            _privRepoMock.Setup(p => p.GetByNameAsync("READ_ONLY", It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new Privilege { PrivilegeId = 1, Name = "READ_ONLY" });
            _roleRepoMock.Setup(r => r.AddAsync(It.IsAny<Role>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _roleRepoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().NotBeNull();
            result.Privileges.Should().ContainSingle(p => p == "READ_ONLY");
        }

        /// <summary>
        /// Test that an exception is thrown if the default "READ_ONLY"
        /// privilege is not found in the database.
        /// </summary>
        [Fact]
        public async Task Handle_ShouldThrow_WhenReadOnlyNotFound()
        {
            var dto = new RoleCreateDto { Name = "Role", Code = "CODE", PrivilegeIds = new List<int>() };
            var command = new CreateRoleCommand(dto);

            _roleRepoMock.Setup(r => r.ExistsByCodeAsync("CODE", It.IsAny<CancellationToken>()))
                         .ReturnsAsync(false);
            _privRepoMock.Setup(p => p.GetByNameAsync("READ_ONLY", It.IsAny<CancellationToken>()))
                         .ReturnsAsync((Privilege?)null);

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<Exception>()
                     .WithMessage("Default READ_ONLY privilege not found in database.");
        }

        /// <summary>
        /// Test that a role is correctly created with valid privilege IDs.
        /// </summary>
        [Fact]
        public async Task Handle_ShouldCreateRole_WithPrivileges()
        {
            var dto = new RoleCreateDto { Name = "Role", Code = "CODE", PrivilegeIds = new List<int> { 1, 2 } };
            var command = new CreateRoleCommand(dto);

            var privileges = new List<Privilege>
            {
                new Privilege { PrivilegeId = 1, Name = "P1" },
                new Privilege { PrivilegeId = 2, Name = "P2" }
            };

            _roleRepoMock.Setup(r => r.ExistsByCodeAsync("CODE", It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _privRepoMock.Setup(p => p.GetPrivilegesByIdsAsync(dto.PrivilegeIds, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(privileges);
            _roleRepoMock.Setup(r => r.AddAsync(It.IsAny<Role>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _roleRepoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Privileges.Should().Contain("P1");
            result.Privileges.Should().Contain("P2");
        }

        /// <summary>
        /// Test that an <see cref="InvalidOperationException"/> is thrown
        /// when the provided privilege IDs are invalid or empty.
        /// </summary>
        [Fact]
        public async Task Handle_ShouldThrow_WhenPrivilegesInvalid()
        {
            var dto = new RoleCreateDto { Name = "Role", Code = "CODE", PrivilegeIds = new List<int> { 99 } };
            var command = new CreateRoleCommand(dto);

            _roleRepoMock.Setup(r => r.ExistsByCodeAsync("CODE", It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _privRepoMock.Setup(p => p.GetPrivilegesByIdsAsync(dto.PrivilegeIds, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new List<Privilege>());

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<InvalidOperationException>()
                     .WithMessage("No valid privileges found for given IDs.");
        }
    }
}
