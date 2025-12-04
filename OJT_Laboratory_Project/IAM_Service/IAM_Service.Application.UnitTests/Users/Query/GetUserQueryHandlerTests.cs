using AutoMapper;
using IAM_Service.Application.DTOs;
using IAM_Service.Application.Interface.IUser;
using IAM_Service.Application.Users.Query;
using IAM_Service.Domain.Entity;
using Moq;

namespace IAM_Service.Application.UnitTests.Users.Query
{
    public class GetUserQueryHandlerTests
    {
        /// <summary>
        /// The mock users repository
        /// </summary>
        private readonly Mock<IUsersRepository> _mockUsersRepository;
        /// <summary>
        /// The mock mapper
        /// </summary>
        private readonly Mock<IMapper> _mockMapper;
        /// <summary>
        /// The handler
        /// </summary>
        private readonly GetUserQueryHandler _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetUserQueryHandlerTests"/> class.
        /// </summary>
        public GetUserQueryHandlerTests()
        {
            _mockUsersRepository = new Mock<IUsersRepository>();
            _mockMapper = new Mock<IMapper>();
            _handler = new GetUserQueryHandler(_mockUsersRepository.Object, _mockMapper.Object);
        }

        /// <summary>
        /// Handles the when users is null should return empty list.
        /// </summary>
        [Fact]
        public async Task Handle_WhenUsersIsNull_ShouldReturnEmptyList()
        {
            // Arrange
            var query = new GetUserQuery();
            _mockUsersRepository.Setup(x => x.GetUserAsync()).ReturnsAsync((List<User>)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        /// <summary>
        /// Handles the when users is empty should return empty list.
        /// </summary>
        [Fact]
        public async Task Handle_WhenUsersIsEmpty_ShouldReturnEmptyList()
        {
            // Arrange
            var query = new GetUserQuery();
            _mockUsersRepository.Setup(x => x.GetUserAsync()).ReturnsAsync(new List<User>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        /// <summary>
        /// Handles the when no filters should return all users.
        /// </summary>
        [Fact]
        public async Task Handle_WhenNoFilters_ShouldReturnAllUsers()
        {
            // Arrange
            var query = new GetUserQuery();
            var users = CreateSampleUsers();
            var userDtos = CreateSampleUserDtos();

            _mockUsersRepository.Setup(x => x.GetUserAsync()).ReturnsAsync(users);
            _mockMapper.Setup(x => x.Map<List<UserDTO>>(users)).Returns(userDtos);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            _mockMapper.Verify(x => x.Map<List<UserDTO>>(users), Times.Once);
        }

        /// <summary>
        /// Handles the when filter by full name should return filtered users.
        /// </summary>
        [Fact]
        public async Task Handle_WhenFilterByFullName_ShouldReturnFilteredUsers()
        {
            // Arrange
            var query = new GetUserQuery
            {
                FilterField = "fullname",
                Keyword = "john"
            };
            var users = CreateSampleUsers();
            var filteredUsers = users.Where(u => u.FullName.ToLower().Contains("john")).ToList();
            var userDtos = CreateSampleUserDtos().Where(u => u.FullName.ToLower().Contains("john")).ToList();

            _mockUsersRepository.Setup(x => x.GetUserAsync()).ReturnsAsync(users);
            _mockMapper.Setup(x => x.Map<List<UserDTO>>(It.IsAny<List<User>>())).Returns(userDtos);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("John Doe", result.First().FullName);
        }

        /// <summary>
        /// Handles the when filter by email should return filtered users.
        /// </summary>
        [Fact]
        public async Task Handle_WhenFilterByEmail_ShouldReturnFilteredUsers()
        {
            // Arrange
            var query = new GetUserQuery
            {
                FilterField = "email",
                Keyword = "jane"
            };
            var users = CreateSampleUsers();
            var userDtos = CreateSampleUserDtos().Where(u => u.Email.ToLower().Contains("jane")).ToList();

            _mockUsersRepository.Setup(x => x.GetUserAsync()).ReturnsAsync(users);
            _mockMapper.Setup(x => x.Map<List<UserDTO>>(It.IsAny<List<User>>())).Returns(userDtos);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("jane.smith@email.com", result.First().Email);
        }

        /// <summary>
        /// Handles the when filter by address should return filtered users.
        /// </summary>
        [Fact]
        public async Task Handle_WhenFilterByAddress_ShouldReturnFilteredUsers()
        {
            // Arrange
            var query = new GetUserQuery
            {
                FilterField = "address",
                Keyword = "hanoi"
            };
            var users = CreateSampleUsers();
            var userDtos = CreateSampleUserDtos().Where(u => u.Address.ToLower().Contains("hanoi")).ToList();

            _mockUsersRepository.Setup(x => x.GetUserAsync()).ReturnsAsync(users);
            _mockMapper.Setup(x => x.Map<List<UserDTO>>(It.IsAny<List<User>>())).Returns(userDtos);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Hanoi, Vietnam", result.First().Address);
        }

        /// <summary>
        /// Handles the when keyword only should search all fields.
        /// </summary>
        [Fact]
        public async Task Handle_WhenKeywordOnly_ShouldSearchAllFields()
        {
            // Arrange
            var query = new GetUserQuery
            {
                Keyword = "john"
            };
            var users = CreateSampleUsers();
            var userDtos = CreateSampleUserDtos().Where(u =>
                u.FullName.ToLower().Contains("john") ||
                u.Email.ToLower().Contains("john") ||
                u.Address.ToLower().Contains("john")).ToList();

            _mockUsersRepository.Setup(x => x.GetUserAsync()).ReturnsAsync(users);
            _mockMapper.Setup(x => x.Map<List<UserDTO>>(It.IsAny<List<User>>())).Returns(userDtos);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("John Doe", result.First().FullName);
        }

        /// <summary>
        /// Handles the when filter by gender should return filtered users.
        /// </summary>
        [Fact]
        public async Task Handle_WhenFilterByGender_ShouldReturnFilteredUsers()
        {
            // Arrange
            var query = new GetUserQuery
            {
                Gender = "male"
            };
            var users = CreateSampleUsers();
            var userDtos = CreateSampleUserDtos().Where(u => u.Gender.ToLower() == "male").ToList();

            _mockUsersRepository.Setup(x => x.GetUserAsync()).ReturnsAsync(users);
            _mockMapper.Setup(x => x.Map<List<UserDTO>>(It.IsAny<List<User>>())).Returns(userDtos);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, u => Assert.Equal("Male", u.Gender));
        }

        /// <summary>
        /// Handles the when filter by minimum age should return filtered users.
        /// </summary>
        [Fact]
        public async Task Handle_WhenFilterByMinAge_ShouldReturnFilteredUsers()
        {
            // Arrange
            var query = new GetUserQuery
            {
                MinAge = 25
            };
            var users = CreateSampleUsers();
            var userDtos = CreateSampleUserDtos().Where(u => u.Age > 25).ToList();

            _mockUsersRepository.Setup(x => x.GetUserAsync()).ReturnsAsync(users);
            _mockMapper.Setup(x => x.Map<List<UserDTO>>(It.IsAny<List<User>>())).Returns(userDtos);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, u => Assert.True(u.Age >= 25));
        }

        /// <summary>
        /// Handles the when filter by maximum age should return filtered users.
        /// </summary>
        [Fact]
        public async Task Handle_WhenFilterByMaxAge_ShouldReturnFilteredUsers()
        {
            // Arrange
            var query = new GetUserQuery
            {
                MaxAge = 30
            };
            var users = CreateSampleUsers();
            var userDtos = CreateSampleUserDtos().Where(u => u.Age <= 30).ToList();

            _mockUsersRepository.Setup(x => x.GetUserAsync()).ReturnsAsync(users);
            _mockMapper.Setup(x => x.Map<List<UserDTO>>(It.IsAny<List<User>>())).Returns(userDtos);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, u => Assert.True(u.Age <= 30));
        }

        /// <summary>
        /// Handles the when sort by email ascending should return sorted users.
        /// </summary>
        [Fact]
        public async Task Handle_WhenSortByEmailAscending_ShouldReturnSortedUsers()
        {
            // Arrange
            var query = new GetUserQuery
            {
                SortBy = "email",
                SortOrder = "asc"
            };
            var users = CreateSampleUsers();
            var userDtos = CreateSampleUserDtos().OrderBy(u => u.Email).ToList();

            _mockUsersRepository.Setup(x => x.GetUserAsync()).ReturnsAsync(users);
            _mockMapper.Setup(x => x.Map<List<UserDTO>>(It.IsAny<List<User>>())).Returns(userDtos);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("bob.wilson@email.com", result.First().Email);
        }

        /// <summary>
        /// Handles the when sort by email descending should return sorted users.
        /// </summary>
        [Fact]
        public async Task Handle_WhenSortByEmailDescending_ShouldReturnSortedUsers()
        {
            // Arrange
            var query = new GetUserQuery
            {
                SortBy = "email",
                SortOrder = "desc"
            };
            var users = CreateSampleUsers();
            var userDtos = CreateSampleUserDtos().OrderByDescending(u => u.Email).ToList();

            _mockUsersRepository.Setup(x => x.GetUserAsync()).ReturnsAsync(users);
            _mockMapper.Setup(x => x.Map<List<UserDTO>>(It.IsAny<List<User>>())).Returns(userDtos);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("john.doe@email.com", result.First().Email);
        }

        /// <summary>
        /// Handles the when sort by age ascending should return sorted users.
        /// </summary>
        [Fact]
        public async Task Handle_WhenSortByAgeAscending_ShouldReturnSortedUsers()
        {
            // Arrange
            var query = new GetUserQuery
            {
                SortBy = "age",
                SortOrder = "ascending"
            };
            var users = CreateSampleUsers();
            var userDtos = CreateSampleUserDtos().OrderBy(u => u.Age).ToList();

            _mockUsersRepository.Setup(x => x.GetUserAsync()).ReturnsAsync(users);
            _mockMapper.Setup(x => x.Map<List<UserDTO>>(It.IsAny<List<User>>())).Returns(userDtos);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal(25, result.First().Age);
        }

        /// <summary>
        /// Handles the when sort by default field ascending should return sorted users.
        /// </summary>
        [Fact]
        public async Task Handle_WhenSortByDefaultFieldAscending_ShouldReturnSortedUsers()
        {
            // Arrange
            var query = new GetUserQuery
            {
                SortOrder = "up"
            };
            var users = CreateSampleUsers();
            var userDtos = CreateSampleUserDtos().OrderBy(u => u.FullName).ToList();

            _mockUsersRepository.Setup(x => x.GetUserAsync()).ReturnsAsync(users);
            _mockMapper.Setup(x => x.Map<List<UserDTO>>(It.IsAny<List<User>>())).Returns(userDtos);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("Bob Wilson", result.First().FullName);
        }

        /// <summary>
        /// Handles the when multiple filters should apply all filters.
        /// </summary>
        [Fact]
        public async Task Handle_WhenMultipleFilters_ShouldApplyAllFilters()
        {
            // Arrange
            var query = new GetUserQuery
            {
                Gender = "male",
                MinAge = 25,
                MaxAge = 35,
                SortBy = "age",
                SortOrder = "asc"
            };
            var users = CreateSampleUsers();
            var userDtos = CreateSampleUserDtos()
                .Where(u => u.Gender.ToLower() == "male" && u.Age >= 25 && u.Age <= 35)
                .OrderBy(u => u.Age)
                .ToList();

            _mockUsersRepository.Setup(x => x.GetUserAsync()).ReturnsAsync(users);
            _mockMapper.Setup(x => x.Map<List<UserDTO>>(It.IsAny<List<User>>())).Returns(userDtos);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, u => Assert.Equal("Male", u.Gender));
            Assert.All(result, u => Assert.True(u.Age >= 25 && u.Age <= 35));
            Assert.Equal(30, result.First().Age);
        }

        /// <summary>
        /// Creates the sample users.
        /// </summary>
        /// <returns></returns>
        private List<User> CreateSampleUsers()
        {
            return new List<User>
            {
                new User
                {
                    UserId = 1,
                    FullName = "John Doe",
                    Email = "john.doe@email.com",
                    PhoneNumber = "1234567890",
                    IdentifyNumber = "123456789",
                    Gender = "Male",
                    Age = 30,
                    Address = "Ho Chi Minh City, Vietnam",
                    DateOfBirth = new DateOnly(1993, 1, 1),
                    Password = "hashedpassword1"
                },
                new User
                {
                    UserId = 2,
                    FullName = "Jane Smith",
                    Email = "jane.smith@email.com",
                    PhoneNumber = "0987654321",
                    IdentifyNumber = "987654321",
                    Gender = "Female",
                    Age = 25,
                    Address = "Hanoi, Vietnam",
                    DateOfBirth = new DateOnly(1998, 5, 15),
                    Password = "hashedpassword2"
                },
                new User
                {
                    UserId = 3,
                    FullName = "Bob Wilson",
                    Email = "bob.wilson@email.com",
                    PhoneNumber = "5555555555",
                    IdentifyNumber = "555555555",
                    Gender = "Male",
                    Age = 35,
                    Address = "Tokyo, Japan",
                    DateOfBirth = new DateOnly(1988, 12, 10),
                    Password = "hashedpassword3"
                }
            };
        }

        /// <summary>
        /// Creates the sample user dtos.
        /// </summary>
        /// <returns></returns>
        private List<UserDTO> CreateSampleUserDtos()
        {
            return new List<UserDTO>
            {
                new UserDTO
                {
                    UserId = 1,
                    FullName = "John Doe",
                    Email = "john.doe@email.com",
                    PhoneNumber = "1234567890",
                    IdentifyNumber = "123456789",
                    Gender = "Male",
                    Age = 30,
                    Address = "Ho Chi Minh City, Vietnam",
                    DateOfBirth = new DateOnly(1993, 1, 1),
                },
                new UserDTO
                {
                    UserId = 2,
                    FullName = "Jane Smith",
                    Email = "jane.smith@email.com",
                    PhoneNumber = "0987654321",
                    IdentifyNumber = "987654321",
                    Gender = "Female",
                    Age = 25,
                    Address = "Hanoi, Vietnam",
                    DateOfBirth = new DateOnly(1998, 5, 15),
                },
                new UserDTO
                {
                    UserId = 3,
                    FullName = "Bob Wilson",
                    Email = "bob.wilson@email.com",
                    PhoneNumber = "5555555555",
                    IdentifyNumber = "555555555",
                    Gender = "Male",
                    Age = 35,
                    Address = "Tokyo, Japan",
                    DateOfBirth = new DateOnly(1988, 12, 10),
                }
            };
        }
    }
}
