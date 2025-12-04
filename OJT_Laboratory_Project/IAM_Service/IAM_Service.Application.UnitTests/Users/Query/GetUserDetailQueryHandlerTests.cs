using AutoMapper;
using IAM_Service.Application.DTOs.User;
using IAM_Service.Application.Interface.IPrivilege;
using IAM_Service.Application.Interface.IRole;
using IAM_Service.Application.Interface.IUser;
using IAM_Service.Application.Users.Query;
using IAM_Service.Domain.Entity;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace IAM_Service.Application.UnitTests.Users.Query
{
    /// <summary>
    /// Unit Test for GetUserDetailQueryHandler
    /// </summary>
    public class GetUserDetailQueryHandlerTests
    {
        private readonly Mock<IUsersRepository> _mockUsersRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IRoleRepository> _mockRoleRepository;
        private readonly Mock<IPrivilegeRepository> _mockPrivilegeRepository;
        private readonly GetUserDetailQueryHandler _handler;

        public GetUserDetailQueryHandlerTests()
        {
            _mockUsersRepository = new Mock<IUsersRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockRoleRepository = new Mock<IRoleRepository>();
            _mockPrivilegeRepository = new Mock<IPrivilegeRepository>();

            _handler = new GetUserDetailQueryHandler(
                _mockUsersRepository.Object,
                _mockMapper.Object,
                _mockRoleRepository.Object,
                _mockPrivilegeRepository.Object);
        }

        private User CreateUser(string email, int? roleId)
        {
            return new User
            {
                Email = email,
                RoleId = roleId
            };
        }

        private Role CreateRole(int roleId, string name, string code)
        {
            return new Role
            {
                RoleId = roleId,
                Name = name,
                Code = code
            };
        }

        [Fact]
        public async Task Handle_WhenUserNotFound_ShouldReturnNull()
        {
            // Arrange
            var query = new GetUserDetailQuery("notfound@example.com");
            _mockUsersRepository
                .Setup(r => r.GetByEmailAsync(query.Email))
                .ReturnsAsync((User)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
            _mockMapper.Verify(m => m.Map<UserDetailDto>(It.IsAny<User>()), Times.Never);
            _mockRoleRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockPrivilegeRepository.Verify(p => p.GetPrivilegeNamesByRoleIdAsync(It.IsAny<int?>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenUserHasRole_ShouldMapAndEnrichRoleAndPrivileges()
        {
            // Arrange
            var email = "user@example.com";
            int roleId = 2;
            var query = new GetUserDetailQuery(email);

            var user = CreateUser(email, roleId);
            _mockUsersRepository
                .Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync(user);

            _mockMapper
                .Setup(m => m.Map<UserDetailDto>(user))
                .Returns(new UserDetailDto());

            var role = CreateRole(roleId, "Admin", "ADMIN");
            var token = new CancellationToken();
            _mockRoleRepository
                .Setup(r => r.GetByIdAsync(roleId, token))
                .ReturnsAsync(role);

            var privileges = new List<string> { "User.Read", "User.Write" };
            _mockPrivilegeRepository
                .Setup(p => p.GetPrivilegeNamesByRoleIdAsync(roleId))
                .ReturnsAsync(privileges);

            // Act
            var result = await _handler.Handle(query, token);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(roleId, result!.RoleId);
            Assert.Equal("Admin", result.RoleName);
            Assert.Equal("ADMIN", result.RoleCode);
            Assert.NotNull(result.Privileges);
            Assert.Equal(2, result.Privileges.Count);
            Assert.Contains("User.Read", result.Privileges);
            Assert.Contains("User.Write", result.Privileges);

            _mockMapper.Verify(m => m.Map<UserDetailDto>(user), Times.Once);
            _mockRoleRepository.Verify(r => r.GetByIdAsync(roleId, token), Times.Once);
            _mockPrivilegeRepository.Verify(p => p.GetPrivilegeNamesByRoleIdAsync(roleId), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenUserHasNoRole_ShouldNotCallRoleRepository_And_PrivilegesEmptyIfNull()
        {
            // Arrange
            var email = "norole@example.com";
            var query = new GetUserDetailQuery(email);

            var user = CreateUser(email, null);
            _mockUsersRepository
                .Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync(user);

            _mockMapper
                .Setup(m => m.Map<UserDetailDto>(user))
                .Returns(new UserDetailDto());

            _mockPrivilegeRepository
                .Setup(p => p.GetPrivilegeNamesByRoleIdAsync(null))
                .ReturnsAsync(new List<string>()); // Trả Task<List<string>>

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.RoleId);
            Assert.Null(result.RoleName);
            Assert.Null(result.RoleCode);
            Assert.NotNull(result.Privileges);
            Assert.Empty(result.Privileges);

            _mockRoleRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockPrivilegeRepository.Verify(p => p.GetPrivilegeNamesByRoleIdAsync(null), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenRoleNotFound_ShouldKeepRoleIdButSetRoleFieldsNull_AndKeepPrivileges()
        {
            // Arrange
            var email = "user2@example.com";
            int roleId = 5;
            var query = new GetUserDetailQuery(email);

            var user = CreateUser(email, roleId);
            _mockUsersRepository
                .Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync(user);

            _mockMapper
                .Setup(m => m.Map<UserDetailDto>(user))
                .Returns(new UserDetailDto());

            _mockRoleRepository
                .Setup(r => r.GetByIdAsync(roleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Role)null);

            var privileges = new List<string> { "Dashboard.View" };
            _mockPrivilegeRepository
                .Setup(p => p.GetPrivilegeNamesByRoleIdAsync(roleId))
                .ReturnsAsync(privileges);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(roleId, result!.RoleId);
            Assert.Null(result.RoleName);
            Assert.Null(result.RoleCode);
            Assert.NotNull(result.Privileges);
            Assert.Single(result.Privileges);
            Assert.Contains("Dashboard.View", result.Privileges);
        }

        [Fact]
        public async Task Handle_WhenRoleIdPresent_ShouldPassCancellationTokenToRoleRepository()
        {
            // Arrange
            var email = "withrole@example.com";
            int roleId = 7;
            var query = new GetUserDetailQuery(email);
            var token = new CancellationToken();

            var user = CreateUser(email, roleId);
            _mockUsersRepository
                .Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync(user);

            _mockMapper
                .Setup(m => m.Map<UserDetailDto>(user))
                .Returns(new UserDetailDto());

            _mockRoleRepository
                .Setup(r => r.GetByIdAsync(roleId, token))
                .ReturnsAsync(CreateRole(roleId, "Manager", "MANAGER"));

            _mockPrivilegeRepository
                .Setup(p => p.GetPrivilegeNamesByRoleIdAsync(roleId))
                .ReturnsAsync(new List<string>());

            // Act
            await _handler.Handle(query, token);

            // Assert
            _mockRoleRepository.Verify(r => r.GetByIdAsync(roleId, token), Times.Once);
        }
    }
}
