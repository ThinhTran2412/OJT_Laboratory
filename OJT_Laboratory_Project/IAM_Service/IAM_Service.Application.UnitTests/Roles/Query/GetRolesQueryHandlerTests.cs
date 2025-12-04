using AutoMapper;
using IAM_Service.Application.DTOs.Pagination;
using IAM_Service.Application.DTOs.Privileges;
using IAM_Service.Application.DTOs.Roles;
using IAM_Service.Application.Interface.IRole;
using IAM_Service.Application.Roles.Query;
using IAM_Service.Domain.Entity;
using Moq;

namespace IAM_Service.Application.UnitTests.Roles.Query
{
    /// <summary>
    /// create usecase test for GetRolesQuery
    /// </summary>
    public class GetRolesQueryHandlerTests
    {
        /// <summary>
        /// The mock role repository
        /// </summary>
        private readonly Mock<IRoleRepository> _mockRoleRepository;

        /// <summary>
        /// The mock mapper
        /// </summary>
        private readonly Mock<IMapper> _mockMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetRolesQueryHandlerTests"/> class.
        /// </summary>
        public GetRolesQueryHandlerTests()
        {
            _mockRoleRepository = new Mock<IRoleRepository>();
            _mockMapper = new Mock<IMapper>();
        }

        /// <summary>
        /// Creates the handler.
        /// </summary>
        /// <returns></returns>
        private GetRolesQueryHandler CreateHandler()
        {
            return new GetRolesQueryHandler(_mockRoleRepository.Object, _mockMapper.Object);
        }

        /// <summary>
        /// Creates the role entity.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="code">The code.</param>
        /// <param name="description">The description.</param>
        /// <param name="privileges">The privileges.</param>
        /// <returns></returns>
        private Role CreateRoleEntity(int roleId, string name, string code, string description, List<Privilege> privileges = null)
        {
            var role = new Role
            {
                RoleId = roleId,
                Name = name,
                Code = code,
                Description = description,
                RolePrivileges = new List<RolePrivilege>()
            };

            if (privileges != null)
            {
                foreach (var privilege in privileges)
                {
                    role.RolePrivileges.Add(new RolePrivilege
                    {
                        Privilege = privilege
                    });
                }
            }

            return role;
        }

        /// <summary>
        /// Creates the privilege entity.
        /// </summary>
        /// <param name="privilegeId">The privilege identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        private Privilege CreatePrivilegeEntity(int privilegeId, string name, string description = "")
        {
            return new Privilege
            {
                PrivilegeId = privilegeId,
                Name = name,
                Description = description
            };
        }

        /// <summary>
        /// Creates the privilege DTO.
        /// </summary>
        /// <param name="privilegeId">The privilege identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        private PrivilegeDto CreatePrivilegeDto(int privilegeId, string name, string description = "")
        {
            return new PrivilegeDto
            {
                PrivilegeId = privilegeId,
                Name = name,
                Description = description
            };
        }

        /// <summary>
        /// Sets up the mapper mock for privilege mapping.
        /// </summary>
        /// <param name="privileges">The privileges.</param>
        /// <param name="privilegeDtos">The privilege DTOs.</param>
        private void SetupMapperMock(List<Privilege> privileges, List<PrivilegeDto> privilegeDtos)
        {
            _mockMapper.Setup(m => m.Map<List<PrivilegeDto>>(It.IsAny<List<Privilege>>()))
                .Returns(privilegeDtos);
        }

        /// <summary>
        /// Creates the paged result.
        /// </summary>
        /// <param name="roles">The roles.</param>
        /// <param name="total">The total.</param>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        private PagedResult<Role> CreatePagedResult(List<Role> roles, int total, int page, int pageSize)
        {
            return new PagedResult<Role>
            {
                Items = roles,
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }

        // --------------------------------------------------------------------------
        // --- TEST CASE: (SUCCESS PATHS) ---
        // --------------------------------------------------------------------------

        /// <summary>
        /// Handles the with valid query returns paged response with roles.
        /// </summary>
        [Fact]
        public async Task Handle_WithValidQuery_ReturnsPagedResponseWithRoles()
        {
            // Arrange
            var handler = CreateHandler();
            var query = new GetRolesQuery
            {
                Search = "admin",
                Page = 1,
                PageSize = 10,
                SortBy = "name",
                SortDesc = false
            };

            // Create privileges
            var readPrivilege = CreatePrivilegeEntity(1, "Read", "Read permission");
            var writePrivilege = CreatePrivilegeEntity(2, "Write", "Write permission");
            var deletePrivilege = CreatePrivilegeEntity(3, "Delete", "Delete permission");

            var roles = new List<Role>
            {
                CreateRoleEntity(1, "Administrator", "ADMIN", "Full system access", 
                    new List<Privilege> { readPrivilege, writePrivilege, deletePrivilege }),
                CreateRoleEntity(2, "User Admin", "USER_ADMIN", "User management", 
                    new List<Privilege> { readPrivilege, writePrivilege })
            };

            // Create privilege DTOs
            var readPrivilegeDto = CreatePrivilegeDto(1, "Read", "Read permission");
            var writePrivilegeDto = CreatePrivilegeDto(2, "Write", "Write permission");
            var deletePrivilegeDto = CreatePrivilegeDto(3, "Delete", "Delete permission");

            var pagedResult = CreatePagedResult(roles, 2, 1, 10);
            _mockRoleRepository.Setup(r => r.GetRolesAsync(
                query.Search,
                query.Page,
                query.PageSize,
                query.SortBy,
                query.SortDesc,
                query.PrivilegeIds,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(pagedResult);

            // Setup mapper mock
            _mockMapper.Setup(m => m.Map<List<PrivilegeDto>>(It.Is<List<Privilege>>(p => p.Count == 3)))
                .Returns(new List<PrivilegeDto> { readPrivilegeDto, writePrivilegeDto, deletePrivilegeDto });
            _mockMapper.Setup(m => m.Map<List<PrivilegeDto>>(It.Is<List<Privilege>>(p => p.Count == 2)))
                .Returns(new List<PrivilegeDto> { readPrivilegeDto, writePrivilegeDto });

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count);
            Assert.Equal(2, result.Total);
            Assert.Equal(1, result.Page);
            Assert.Equal(10, result.PageSize);
            Assert.Null(result.Message);

            var firstRole = result.Items[0];
            Assert.Equal(1, firstRole.RoleId);
            Assert.Equal("Administrator", firstRole.Name);
            Assert.Equal("ADMIN", firstRole.Code);
            Assert.Equal("Full system access", firstRole.Description);
            Assert.Equal(3, firstRole.Privileges.Count);
            Assert.Contains(firstRole.Privileges, p => p.Name == "Read");
            Assert.Contains(firstRole.Privileges, p => p.Name == "Write");
            Assert.Contains(firstRole.Privileges, p => p.Name == "Delete");

            var secondRole = result.Items[1];
            Assert.Equal(2, secondRole.RoleId);
            Assert.Equal("User Admin", secondRole.Name);
            Assert.Equal("USER_ADMIN", secondRole.Code);
            Assert.Equal("User management", secondRole.Description);
            Assert.Equal(2, secondRole.Privileges.Count);
            Assert.Contains(secondRole.Privileges, p => p.Name == "Read");
            Assert.Contains(secondRole.Privileges, p => p.Name == "Write");
        }

        /// <summary>
        /// Handles the with empty roles returns paged response with no data message.
        /// </summary>
        [Fact]
        public async Task Handle_WithEmptyRoles_ReturnsPagedResponseWithNoDataMessage()
        {
            // Arrange
            var handler = CreateHandler();
            var query = new GetRolesQuery
            {
                Page = 1,
                PageSize = 10
            };

            var emptyRoles = new List<Role>();
            var pagedResult = CreatePagedResult(emptyRoles, 0, 1, 10);
            _mockRoleRepository.Setup(r => r.GetRolesAsync(
                query.Search,
                query.Page,
                query.PageSize,
                query.SortBy,
                query.SortDesc,
                query.PrivilegeIds,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Items);
            Assert.Equal(0, result.Total);
            Assert.Equal(1, result.Page);
            Assert.Equal(10, result.PageSize);
            Assert.Equal("No Data", result.Message);
        }

        /// <summary>
        /// Handles the with roles without privileges returns roles with empty privileges.
        /// </summary>
        [Fact]
        public async Task Handle_WithRolesWithoutPrivileges_ReturnsRolesWithEmptyPrivileges()
        {
            // Arrange
            var handler = CreateHandler();
            var query = new GetRolesQuery();

            var roles = new List<Role>
            {
                CreateRoleEntity(1, "Basic Role", "BASIC", "Basic role without privileges", null)
            };

            var pagedResult = CreatePagedResult(roles, 1, 1, 10);
            _mockRoleRepository.Setup(r => r.GetRolesAsync(
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<List<int>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(pagedResult);

            // Setup mapper mock for empty privileges
            _mockMapper.Setup(m => m.Map<List<PrivilegeDto>>(It.IsAny<List<Privilege>>()))
                .Returns(new List<PrivilegeDto>());

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            var role = result.Items[0];
            Assert.Equal(1, role.RoleId);
            Assert.Equal("Basic Role", role.Name);
            Assert.Equal("BASIC", role.Code);
            Assert.Equal("Basic role without privileges", role.Description);
            Assert.Empty(role.Privileges);
        }

        /// <summary>
        /// Handles the with empty role privileges returns roles with empty privileges.
        /// </summary>
        [Fact]
        public async Task Handle_WithEmptyRolePrivileges_ReturnsRolesWithEmptyPrivileges()
        {
            // Arrange
            var handler = CreateHandler();
            var query = new GetRolesQuery();

            var role = new Role
            {
                RoleId = 1,
                Name = "Test Role",
                Code = "TEST",
                Description = "Test role",
                RolePrivileges = new List<RolePrivilege>() // Empty list instead of null
            };

            var roles = new List<Role> { role };
            var pagedResult = CreatePagedResult(roles, 1, 1, 10);
            _mockRoleRepository.Setup(r => r.GetRolesAsync(
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<List<int>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(pagedResult);

            // Setup mapper mock for empty privileges
            _mockMapper.Setup(m => m.Map<List<PrivilegeDto>>(It.IsAny<List<Privilege>>()))
                .Returns(new List<PrivilegeDto>());

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            var resultRole = result.Items[0];
            Assert.Equal(1, resultRole.RoleId);
            Assert.Equal("Test Role", resultRole.Name);
            Assert.Equal("TEST", resultRole.Code);
            Assert.Equal("Test role", resultRole.Description);
            Assert.Empty(resultRole.Privileges);
        }

        // --------------------------------------------------------------------------
        // --- TEST CASE: CHECK PARAMETERS ARE PASSED CORRECTLY ---
        // --------------------------------------------------------------------------

        /// <summary>
        /// Handles the passes correct parameters to repository.
        /// </summary>
        [Fact]
        public async Task Handle_PassesCorrectParametersToRepository()
        {
            // Arrange
            var handler = CreateHandler();
            var query = new GetRolesQuery
            {
                Search = "test search",
                Page = 2,
                PageSize = 5,
                SortBy = "code",
                SortDesc = true,
                PrivilegeIds = new List<int> { 1, 2, 3 }
            };

            var emptyResult = CreatePagedResult(new List<Role>(), 0, 2, 5);
            _mockRoleRepository.Setup(r => r.GetRolesAsync(
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<List<int>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyResult);

            // Act
            await handler.Handle(query, CancellationToken.None);

            // Assert
            _mockRoleRepository.Verify(r => r.GetRolesAsync(
                "test search",
                2,
                5,
                "code",
                true,
                new List<int> { 1, 2, 3 },
                It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Handles the with default query values passes correct defaults to repository.
        /// </summary>
        [Fact]
        public async Task Handle_WithDefaultQueryValues_PassesCorrectDefaultsToRepository()
        {
            // Arrange
            var handler = CreateHandler();
            var query = new GetRolesQuery();

            var emptyResult = CreatePagedResult(new List<Role>(), 0, 1, 10);
            _mockRoleRepository.Setup(r => r.GetRolesAsync(
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<List<int>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyResult);

            // Act
            await handler.Handle(query, CancellationToken.None);

            // Assert
            _mockRoleRepository.Verify(r => r.GetRolesAsync(
                null, // Search default
                1,    // Page default
                10,   // PageSize default
                null, // SortBy default
                false, // SortDesc default
                null, // PrivilegeIds default
                It.IsAny<CancellationToken>()), Times.Once);
        }

        // --------------------------------------------------------------------------
        // --- TEST CASE: TEST CANCELLATION TOKEN ---
        // --------------------------------------------------------------------------

        /// <summary>
        /// Handles the with cancellation token passes token to repository.
        /// </summary>
        [Fact]
        public async Task Handle_WithCancellationToken_PassesTokenToRepository()
        {
            // Arrange
            var handler = CreateHandler();
            var query = new GetRolesQuery();
            var cancellationToken = new CancellationToken();

            var emptyResult = CreatePagedResult(new List<Role>(), 0, 1, 10);
            _mockRoleRepository.Setup(r => r.GetRolesAsync(
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<List<int>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyResult);

            // Act
            await handler.Handle(query, cancellationToken);

            // Assert
            _mockRoleRepository.Verify(r => r.GetRolesAsync(
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<List<int>>(),
                cancellationToken), Times.Once);
        }

        // --------------------------------------------------------------------------
        // --- TEST CASE: TEST MAPPING COMPLEX ---
        // --------------------------------------------------------------------------

        /// <summary>
        /// Handles the with multiple roles and privileges maps correctly.
        /// </summary>
        [Fact]
        public async Task Handle_WithMultipleRolesAndPrivileges_MapsCorrectly()
        {
            // Arrange
            var handler = CreateHandler();
            var query = new GetRolesQuery();

            // Create privileges
            var readPrivilege = CreatePrivilegeEntity(1, "Read", "Read permission");
            var writePrivilege = CreatePrivilegeEntity(2, "Write", "Write permission");
            var deletePrivilege = CreatePrivilegeEntity(3, "Delete", "Delete permission");
            var adminPrivilege = CreatePrivilegeEntity(4, "Admin", "Admin permission");

            var roles = new List<Role>
            {
                CreateRoleEntity(1, "Super Admin", "SUPER_ADMIN", "Super administrator",
                    new List<Privilege> { readPrivilege, writePrivilege, deletePrivilege, adminPrivilege }),
                CreateRoleEntity(2, "Moderator", "MODERATOR", "Content moderator",
                    new List<Privilege> { readPrivilege, writePrivilege }),
                CreateRoleEntity(3, "Viewer", "VIEWER", "Read-only access",
                    new List<Privilege> { readPrivilege })
            };

            // Create privilege DTOs
            var readPrivilegeDto = CreatePrivilegeDto(1, "Read", "Read permission");
            var writePrivilegeDto = CreatePrivilegeDto(2, "Write", "Write permission");
            var deletePrivilegeDto = CreatePrivilegeDto(3, "Delete", "Delete permission");
            var adminPrivilegeDto = CreatePrivilegeDto(4, "Admin", "Admin permission");

            var pagedResult = CreatePagedResult(roles, 3, 1, 10);
            _mockRoleRepository.Setup(r => r.GetRolesAsync(
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<List<int>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(pagedResult);

            // Setup mapper mock for different privilege counts
            _mockMapper.Setup(m => m.Map<List<PrivilegeDto>>(It.Is<List<Privilege>>(p => p.Count == 4)))
                .Returns(new List<PrivilegeDto> { readPrivilegeDto, writePrivilegeDto, deletePrivilegeDto, adminPrivilegeDto });
            _mockMapper.Setup(m => m.Map<List<PrivilegeDto>>(It.Is<List<Privilege>>(p => p.Count == 2)))
                .Returns(new List<PrivilegeDto> { readPrivilegeDto, writePrivilegeDto });
            _mockMapper.Setup(m => m.Map<List<PrivilegeDto>>(It.Is<List<Privilege>>(p => p.Count == 1)))
                .Returns(new List<PrivilegeDto> { readPrivilegeDto });

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Items.Count);

            // check Super Admin
            var superAdmin = result.Items[0];
            Assert.Equal(1, superAdmin.RoleId);
            Assert.Equal("Super Admin", superAdmin.Name);
            Assert.Equal("SUPER_ADMIN", superAdmin.Code);
            Assert.Equal("Super administrator", superAdmin.Description);
            Assert.Equal(4, superAdmin.Privileges.Count);
            Assert.Contains(superAdmin.Privileges, p => p.Name == "Read");
            Assert.Contains(superAdmin.Privileges, p => p.Name == "Write");
            Assert.Contains(superAdmin.Privileges, p => p.Name == "Delete");
            Assert.Contains(superAdmin.Privileges, p => p.Name == "Admin");

            // check Moderator
            var moderator = result.Items[1];
            Assert.Equal(2, moderator.RoleId);
            Assert.Equal("Moderator", moderator.Name);
            Assert.Equal("MODERATOR", moderator.Code);
            Assert.Equal("Content moderator", moderator.Description);
            Assert.Equal(2, moderator.Privileges.Count);
            Assert.Contains(moderator.Privileges, p => p.Name == "Read");
            Assert.Contains(moderator.Privileges, p => p.Name == "Write");

            // check Viewer
            var viewer = result.Items[2];
            Assert.Equal(3, viewer.RoleId);
            Assert.Equal("Viewer", viewer.Name);
            Assert.Equal("VIEWER", viewer.Code);
            Assert.Equal("Read-only access", viewer.Description);
            Assert.Single(viewer.Privileges);
            Assert.Contains(viewer.Privileges, p => p.Name == "Read");
        }
    }
}
