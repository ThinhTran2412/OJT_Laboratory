using AutoMapper;
using IAM_Service.Application.Common.Mappings;
using IAM_Service.Application.DTOs;
using IAM_Service.Application.DTOs.Privileges;
using IAM_Service.Application.Users.Command;
using IAM_Service.Domain.Entity;
using Xunit;

namespace IAM_Service.Application.UnitTests.AutoMapper
{
    /// <summary>
    /// Unit tests for <see cref="AutoMapperProfile"/>.
    /// Validates mapping between domain entities and DTOs.
    /// </summary>
    public class AutoMapperProfileTests
    {
        private readonly IConfigurationProvider _configuration;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMapperProfileTests"/> class.
        /// Sets up AutoMapper configuration for testing.
        /// </summary>
        public AutoMapperProfileTests()
        {
            _configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperProfile>();
            });
            _mapper = _configuration.CreateMapper();
        }

        /// <summary>
        /// Ensures AutoMapper configuration is valid.
        /// </summary>
        [Fact]
        public void AutoMapper_Configuration_IsValid()
        {
            _configuration.AssertConfigurationIsValid();
        }

        /// <summary>
        /// Tests mapping from <see cref="CreateUserCommand"/> to <see cref="User"/>.
        /// </summary>
        [Fact]
        public void Should_Map_CreateUserCommand_To_User()
        {
            var command = new CreateUserCommand
            {
                FullName = "John Doe",
                Email = "john@example.com",
                PhoneNumber = "123456789",
                IdentifyNumber = "ID123",
                Gender = "Male",
                Age = 30,
                Address = "123 Street",
                DateOfBirth = new DateOnly(1993, 1, 1),
                RoleId = 1
            };

            var user = _mapper.Map<User>(command);

            Assert.Equal(command.FullName, user.FullName);
            Assert.Equal(command.Email, user.Email);
            Assert.Equal(command.PhoneNumber, user.PhoneNumber);
            Assert.Equal(command.IdentifyNumber, user.IdentifyNumber);
            Assert.Equal(command.Gender, user.Gender);
            Assert.Equal(command.Age, user.Age);
            Assert.Equal(command.Address, user.Address);
            Assert.Equal(command.DateOfBirth, user.DateOfBirth);
            Assert.Equal(command.RoleId, user.RoleId);
        }

        /// <summary>
        /// Tests mapping from <see cref="User"/> to <see cref="UserDTO"/>.
        /// </summary>
        [Fact]
        public void Should_Map_User_To_UserDTO()
        {
            var user = new User
            {
                UserId = 1,
                FullName = "John Doe",
                Email = "john@example.com"
            };

            var dto = _mapper.Map<UserDTO>(user);

            Assert.Equal(user.UserId, dto.UserId);
            Assert.Equal(user.FullName, dto.FullName);
            Assert.Equal(user.Email, dto.Email);
        }

        /// <summary>
        /// Tests mapping from <see cref="Privilege"/> to <see cref="PrivilegeDto"/>.
        /// </summary>
        [Fact]
        public void Should_Map_Privilege_To_PrivilegeDto()
        {
            var privilege = new Privilege
            {
                PrivilegeId = 1,
                Name = "Admin",
                Description = "Full access"
            };

            var dto = _mapper.Map<PrivilegeDto>(privilege);

            Assert.Equal(privilege.PrivilegeId, dto.PrivilegeId);
            Assert.Equal(privilege.Name, dto.Name);
            Assert.Equal(privilege.Description, dto.Description);
        }
    }
}
