using AutoMapper;
using IAM_Service.Application.DTOs;
using IAM_Service.Application.Interface.IUser;
using IAM_Service.Application.Users.Query;
using IAM_Service.Domain.Entity; // namespace domain entity của bạn
using Moq;

namespace IAM_Service.Application.UnitTests.Users.Query
{
    /// <summary>
    /// 
    /// </summary>
    public class GetUserProfileQueryHandlerTests
    {
        /// <summary>
        /// The mock user repo
        /// </summary>
        private readonly Mock<IUsersRepository> _mockUserRepo;
        /// <summary>
        /// The mock mapper
        /// </summary>
        private readonly Mock<IMapper> _mockMapper;
        /// <summary>
        /// The handler
        /// </summary>
        private readonly GetUserProfileQueryHandler _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetUserProfileQueryHandlerTests"/> class.
        /// </summary>
        public GetUserProfileQueryHandlerTests()
        {
            _mockUserRepo = new Mock<IUsersRepository>();
            _mockMapper = new Mock<IMapper>();
            _handler = new GetUserProfileQueryHandler(_mockUserRepo.Object, _mockMapper.Object);
        }

        /// <summary>
        /// Handles the should return user profile dto when user exists.
        /// </summary>
        [Fact]
        public async Task Handle_ShouldReturnUserProfileDTO_WhenUserExists()
        {
            // Arrange
            var userId = 1;
            var userEntity = new User
            {
                UserId = userId,
                FullName = "John Doe",
                PhoneNumber = "0123456789",
                Gender = "Male",
                Age = 30,
                Address = "123 Main St",
                DateOfBirth = new DateOnly(1995, 5, 10)
            };

            var userDto = new UserProfileDTO
            {
                UserId = userId,
                FullName = "John Doe",
                PhoneNumber = "0123456789",
                Gender = "Male",
                Age = 30,
                Address = "123 Main St",
                DateOfBirth = new DateOnly(1995, 5, 10)
            };

            _mockUserRepo
                .Setup(repo => repo.GetByUserIdAsync(userId))
                .ReturnsAsync(userEntity);

            _mockMapper
                .Setup(mapper => mapper.Map<UserProfileDTO>(userEntity))
                .Returns(userDto);

            var query = new GetUserProfileQuery(userId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Equal("John Doe", result.FullName);
            Assert.Equal("Male", result.Gender);

            _mockUserRepo.Verify(r => r.GetByUserIdAsync(userId), Times.Once);
            _mockMapper.Verify(m => m.Map<UserProfileDTO>(userEntity), Times.Once);
        }

        /// <summary>
        /// Handles the should throw key not found exception when user not found.
        /// </summary>
        [Fact]
        public async Task Handle_ShouldThrowKeyNotFoundException_WhenUserNotFound()
        {
            // Arrange
            var userId = 999;

            _mockUserRepo
                .Setup(repo => repo.GetByUserIdAsync(userId))
                .ReturnsAsync((User)null);

            var query = new GetUserProfileQuery(userId);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _handler.Handle(query, CancellationToken.None));

            Assert.Equal($"User with ID {userId} not found.", ex.Message);
            _mockUserRepo.Verify(r => r.GetByUserIdAsync(userId), Times.Once);
            _mockMapper.Verify(m => m.Map<UserProfileDTO>(It.IsAny<User>()), Times.Never);
        }
    }
}
