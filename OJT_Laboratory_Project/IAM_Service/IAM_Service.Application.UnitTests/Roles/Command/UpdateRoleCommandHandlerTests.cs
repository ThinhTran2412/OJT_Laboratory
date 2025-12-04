using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using IAM_Service.Application.Roles.Command;
using IAM_Service.Application.DTOs.Roles;
using IAM_Service.Application.Interface.IRole;
using IAM_Service.Application.Interface.IPrivilege;
using IAM_Service.Application.Interface.IAuditLogRepository;
using IAM_Service.Domain.Entity;

namespace IAM_Service.Application.UnitTests.Roles.Command
{
    public class UpdateRoleCommandHandlerTests
    {
        private readonly Mock<IRoleCommandRepository> _roleCommandRepoMock;
        private readonly Mock<IPrivilegeRepository> _privilegeRepoMock;
        private readonly Mock<IAuditLogRepository> _auditLogRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<UpdateRoleCommandHandler>> _loggerMock;
        private readonly UpdateRoleCommandHandler _handler;
        private readonly Mock<IPrivilegeNormalizationService> _privilegeNormalizationServiceMock;

        public UpdateRoleCommandHandlerTests()
        {
            _roleCommandRepoMock = new Mock<IRoleCommandRepository>();
            _privilegeRepoMock = new Mock<IPrivilegeRepository>();
            _auditLogRepoMock = new Mock<IAuditLogRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<UpdateRoleCommandHandler>>();
            _privilegeNormalizationServiceMock = new Mock<IPrivilegeNormalizationService>();

            _handler = new UpdateRoleCommandHandler(
                _roleCommandRepoMock.Object,
                _privilegeRepoMock.Object,
                _auditLogRepoMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _privilegeNormalizationServiceMock.Object
            );
        }

        [Fact]
        public async Task Handle_Should_Update_Role_Successfully()
        {
            // ARRANGE
            var roleId = 1;
            var privilegeIds = new List<int> { 10, 20 };

            var dto = new UpdateRoleDto
            {
                Name = "Manager",
                Description = "Updated role",
                PrivilegeIds = privilegeIds
            };

            var command = new UpdateRoleCommand(roleId, dto);

            var existingRole = new Role
            {
                RoleId = roleId,
                Name = "Old Role",
                Description = "Old desc",
                RolePrivileges = new List<RolePrivilege>()
            };

            var privileges = new List<Privilege>
            {
                new Privilege { PrivilegeId = 10, Name = "CanView" },
                new Privilege { PrivilegeId = 20, Name = "CanEdit" }
            };

            _roleCommandRepoMock
                .Setup(r => r.GetByIdWithTrackingAsync(roleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingRole);

            _privilegeRepoMock
                .Setup(p => p.GetPrivilegesByIdsAsync(privilegeIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync(privileges);

            _mapperMock
                .Setup(m => m.Map<RoleDto>(existingRole))
                .Returns(new RoleDto { RoleId = roleId, Name = dto.Name });

            // ACT
            var result = await _handler.Handle(command, CancellationToken.None);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(dto.Name, result.Name);

            _roleCommandRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _auditLogRepoMock.Verify(a => a.AddAsync(It.IsAny<AuditLog>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Throw_When_Role_Not_Found()
        {
            // ARRANGE
            var command = new UpdateRoleCommand(999, new UpdateRoleDto { Name = "Unknown" });
            _roleCommandRepoMock
                .Setup(r => r.GetByIdWithTrackingAsync(command.RoleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Role)null);

            // ACT & ASSERT
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_Should_Throw_When_Privilege_Invalid()
        {
            // ARRANGE
            var roleId = 2;
            var privilegeIds = new List<int> { 1, 2, 3 };

            var dto = new UpdateRoleDto
            {
                Name = "Tester",
                Description = "Invalid privileges",
                PrivilegeIds = privilegeIds
            };

            var command = new UpdateRoleCommand(roleId, dto);

            var existingRole = new Role
            {
                RoleId = roleId,
                Name = "Old Role",
                RolePrivileges = new List<RolePrivilege>()
            };

            _roleCommandRepoMock
                .Setup(r => r.GetByIdWithTrackingAsync(roleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingRole);

            // giả sử chỉ tìm được 2 trong 3 privilege
            _privilegeRepoMock
                .Setup(p => p.GetPrivilegesByIdsAsync(privilegeIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Privilege>
                {
                    new Privilege { PrivilegeId = 1, Name = "CanView" },
                    new Privilege { PrivilegeId = 2, Name = "CanEdit" }
                });

            // ACT & ASSERT
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_Should_Log_Audit_After_Update()
        {
            // ARRANGE
            var roleId = 3;
            var dto = new UpdateRoleDto
            {
                Name = "AuditedRole",
                Description = "Desc",
                PrivilegeIds = new List<int> { 5 }
            };
            var command = new UpdateRoleCommand(roleId, dto);

            var existingRole = new Role
            {
                RoleId = roleId,
                Name = "Old",
                RolePrivileges = new List<RolePrivilege>()
            };

            var privilege = new Privilege { PrivilegeId = 5, Name = "CanView" };

            _roleCommandRepoMock
                .Setup(r => r.GetByIdWithTrackingAsync(roleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingRole);

            _privilegeRepoMock
                .Setup(p => p.GetPrivilegesByIdsAsync(dto.PrivilegeIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Privilege> { privilege });

            _mapperMock
                .Setup(m => m.Map<RoleDto>(existingRole))
                .Returns(new RoleDto { RoleId = roleId, Name = dto.Name });

            // ACT
            var result = await _handler.Handle(command, CancellationToken.None);

            // ASSERT
            Assert.NotNull(result);
            _auditLogRepoMock.Verify(a => a.AddAsync(It.Is<AuditLog>(
                log => log.Action == "Update" &&
                       log.EntityName == "Role" &&
                       log.Changes.Contains("AuditedRole")
            ), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
