using AutoMapper;
using FluentAssertions;
using IAM_Service.Application.AuditLogs.Querry;
using IAM_Service.Application.DTOs;
using IAM_Service.Application.Interface.IAuditLogRepository;
using IAM_Service.Domain.Entity;
using Moq;

namespace IAM_Service.Application.UnitTests.Audit_Log
{
    /// <summary>
    /// Unit tests for the <see cref="GetAuditLogQueryHandler"/> class.
    /// Ensures that audit logs are correctly retrieved and mapped to DTOs.
    /// </summary>
    public class GetAuditLogQueryHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IAuditLogRepository> _repositoryMock;
        private readonly GetAuditLogQueryHandler _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAuditLogQueryHandlerTests"/> class.
        /// Sets up mocks and the handler before each test.
        /// </summary>
        public GetAuditLogQueryHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _repositoryMock = new Mock<IAuditLogRepository>();
            _handler = new GetAuditLogQueryHandler(_mapperMock.Object, _repositoryMock.Object);
        }

        /// <summary>
        /// Verifies that the handler correctly returns a mapped list of <see cref="AuditLogDto"/> objects.
        /// </summary>
        [Fact]
        public async Task Handle_Should_Return_Mapped_List_Of_AuditLogDto()
        {
            // Arrange
            var logs = new List<AuditLog>
            {
                new AuditLog
                {
                    Id = 1,
                    Action = "Create",
                    UserEmail = "user1@example.com",
                    EntityName = "User",
                    Timestamp = DateTime.UtcNow,
                    Changes = "Created new user"
                },
                new AuditLog
                {
                    Id = 2,
                    Action = "Delete",
                    UserEmail = "user2@example.com",
                    EntityName = "Role",
                    Timestamp = DateTime.UtcNow,
                    Changes = "Deleted old role"
                }
            };

            var mappedDtos = new List<AuditLogDto>
            {
                new AuditLogDto
                {
                    Id = 1,
                    Action = "Create",
                    UserEmail = "user1@example.com",
                    EntityName = "User",
                    Timestamp = DateTime.UtcNow,
                    Changes = "Created new user"
                },
                new AuditLogDto
                {
                    Id = 2,
                    Action = "Delete",
                    UserEmail = "user2@example.com",
                    EntityName = "Role",
                    Timestamp = DateTime.UtcNow,
                    Changes = "Deleted old role"
                }
            };

            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(logs);
            _mapperMock.Setup(m => m.Map<List<AuditLogDto>>(logs)).Returns(mappedDtos);

            var query = new GetAuditLogQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull("the handler should always return a list of audit logs");
            result.Should().HaveCount(2, "two logs were returned from the repository");
            result.Should().BeEquivalentTo(mappedDtos, "the mapper must map entities correctly");

            _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
            _mapperMock.Verify(m => m.Map<List<AuditLogDto>>(logs), Times.Once);
        }

        /// <summary>
        /// Ensures that the handler returns an empty list when no audit logs are found in the repository.
        /// </summary>
        [Fact]
        public async Task Handle_Should_Return_Empty_List_When_No_Logs_Found()
        {
            // Arrange
            var emptyLogs = new List<AuditLog>();
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(emptyLogs);
            _mapperMock.Setup(m => m.Map<List<AuditLogDto>>(emptyLogs)).Returns(new List<AuditLogDto>());

            var query = new GetAuditLogQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull("the handler should never return null");
            result.Should().BeEmpty("no logs were returned from the repository");

            _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
            _mapperMock.Verify(m => m.Map<List<AuditLogDto>>(emptyLogs), Times.Once);
        }
    }
}
