using AutoMapper;
using IAM_Service.Application.Interface.IEmailSender;
using IAM_Service.Application.Interface.IPasswordHasher;
using IAM_Service.Application.Interface.IUser;
using IAM_Service.Application.Interface.IRole;
using IAM_Service.Application.Users.Command;
using IAM_Service.Domain.Entity;
using MediatR;
using Moq;

namespace IAM_Service.Application.Tests.Users.Command
{
    /// <summary>
    /// Unit tests for CreateUserCommandHandler
    /// </summary>
    public class CreateUserCommandHandlerTests
    {
        private readonly Mock<IUsersRepository> _mockUserRepository;
        private readonly Mock<IRoleRepository> _mockRoleRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IPasswordHasher> _mockPasswordHasher;
        private readonly Mock<IEmailSender> _mockEmailSender;
        private readonly CreateUserCommandHandler _handler;

        public CreateUserCommandHandlerTests()
        {
            _mockUserRepository = new Mock<IUsersRepository>();
            _mockRoleRepository = new Mock<IRoleRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockPasswordHasher = new Mock<IPasswordHasher>();
            _mockEmailSender = new Mock<IEmailSender>();

            _handler = new CreateUserCommandHandler(
                _mockUserRepository.Object,
                _mockRoleRepository.Object,
                _mockMapper.Object,
                _mockPasswordHasher.Object,
                _mockEmailSender.Object
            );
        }

        [Fact]
        public async Task Handle_ValidCommand_ShouldCallAllDependenciesCorrectly()
        {
            // ARRANGE
            var command = new CreateUserCommand
            {
                Email = "testuser@gmail.com",
                Address = "HCM",
                Age = 25,
                DateOfBirth = new DateOnly(1998, 1, 1),
                FullName = "Test User",
                Gender = "Male",
                IdentifyNumber = "123456789",
                PhoneNumber = "0123456789"
            };

            var newUser = new User { Email = command.Email };
            var fakeHashedPassword = "hashed_password";

            _mockUserRepository.Setup(r => r.GetByEmailAsync(command.Email))
                               .ReturnsAsync((User?)null);

            _mockRoleRepository.Setup(r => r.GetByCodeAsync("READ_ONLY", It.IsAny<CancellationToken>()))
                               .ReturnsAsync((Role?)null);

            _mockMapper.Setup(m => m.Map<User>(command))
                       .Returns(newUser);

            _mockPasswordHasher.Setup(p => p.Hash(It.IsAny<string>()))
                               .Returns(fakeHashedPassword);

            _mockUserRepository.Setup(r => r.CreateUser(It.IsAny<User>()))
                               .Returns(Task.CompletedTask);

            // Fix CS0854: không dùng named arguments khi mock
            _mockEmailSender.Setup(e => e.SendEmail(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()
            )).Verifiable();

            // ACT
            var result = await _handler.Handle(command, CancellationToken.None);

            // ASSERT
            Assert.Equal(Unit.Value, result);

            _mockMapper.Verify(m => m.Map<User>(command), Times.Once);
            _mockPasswordHasher.Verify(p => p.Hash(It.IsAny<string>()), Times.Once);
            _mockUserRepository.Verify(r => r.CreateUser(It.Is<User>(u => u.Password == fakeHashedPassword)), Times.Once);
            _mockEmailSender.Verify(e => e.SendEmail(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()
            ), Times.Once);
        }

        [Fact]
        public async Task Handle_DuplicateEmail_ShouldThrowInvalidOperationException()
        {
            // ARRANGE
            var command = new CreateUserCommand { Email = "existing@gmail.com" };
            var existingUser = new User { Email = command.Email };

            _mockUserRepository.Setup(r => r.GetByEmailAsync(command.Email))
                               .ReturnsAsync(existingUser);

            // ACT & ASSERT
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(command, CancellationToken.None)
            );

            _mockUserRepository.Verify(r => r.CreateUser(It.IsAny<User>()), Times.Never);
            _mockEmailSender.Verify(e => e.SendEmail(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()
            ), Times.Never);
        }

        [Fact]
        public async Task Handle_WithRoleId_ShouldAssignRoleCorrectly()
        {
            // ARRANGE
            var command = new CreateUserCommand
            {
                Email = "roleuser@gmail.com",
                RoleId = 99
            };
            var newUser = new User { Email = command.Email };
            var role = new Role { RoleId = 99 };

            _mockUserRepository.Setup(r => r.GetByEmailAsync(command.Email))
                               .ReturnsAsync((User?)null);

            _mockRoleRepository.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
                               .ReturnsAsync(role);

            _mockMapper.Setup(m => m.Map<User>(command)).Returns(newUser);
            _mockPasswordHasher.Setup(p => p.Hash(It.IsAny<string>())).Returns("hashed");
            _mockUserRepository.Setup(r => r.CreateUser(It.IsAny<User>())).Returns(Task.CompletedTask);
            _mockEmailSender.Setup(e => e.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                                                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();

            // ACT
            var result = await _handler.Handle(command, CancellationToken.None);

            // ASSERT
            Assert.Equal(Unit.Value, result);
            Assert.Equal(99, newUser.RoleId);
        }

        [Fact]
        public async Task Handle_WithInvalidRoleId_ShouldThrowInvalidOperationException()
        {
            // ARRANGE
            var command = new CreateUserCommand
            {
                Email = "user@gmail.com",
                RoleId = 99
            };

            _mockUserRepository.Setup(r => r.GetByEmailAsync(command.Email))
                            .ReturnsAsync((User?)null);

            _mockRoleRepository.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
                            .ReturnsAsync((Role?)null); // role không tồn tại

            // ACT & ASSERT
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _handler.Handle(command, CancellationToken.None)
            );
        }

        [Fact]
        public async Task Handle_WithoutRoleId_ShouldAssignDefaultRoleIfExists()
        {
            // ARRANGE
            var command = new CreateUserCommand
            {
                Email = "defaultrole@gmail.com",
                RoleId = null
            };

            var newUser = new User { Email = command.Email };
            var defaultRole = new Role { RoleId = 10 };

            _mockUserRepository.Setup(r => r.GetByEmailAsync(command.Email))
                            .ReturnsAsync((User?)null);

            _mockRoleRepository.Setup(r => r.GetByCodeAsync("READ_ONLY", It.IsAny<CancellationToken>()))
                            .ReturnsAsync(defaultRole);

            _mockMapper.Setup(m => m.Map<User>(command)).Returns(newUser);
            _mockPasswordHasher.Setup(p => p.Hash(It.IsAny<string>())).Returns("hashed");
            _mockUserRepository.Setup(r => r.CreateUser(It.IsAny<User>())).Returns(Task.CompletedTask);
            _mockEmailSender.Setup(e => e.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                                                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();

            // ACT
            var result = await _handler.Handle(command, CancellationToken.None);

            // ASSERT
            Assert.Equal(Unit.Value, result);
            Assert.Equal(defaultRole.RoleId, newUser.RoleId); // đảm bảo default role được gán
        }
    }
}
