using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IAM_Service.Application.DTOs.Privileges;
using IAM_Service.Application.Interface.IPrivilege;
using IAM_Service.Application.Privileges.Query;
using Moq;
using Xunit;

namespace IAM_Service.Application.UnitTests.Privileges.Query
{
    /// <summary>
    /// Unit tests for <see cref="GetPrivilegesQueryHandler"/>.
    /// Verifies that privileges are correctly retrieved and mapped to DTOs.
    /// </summary>
    public class GetPrivilegesQueryHandlerTests
    {
        /// <summary>
        /// Tests that the <see cref="GetPrivilegesQueryHandler.Handle"/> method
        /// returns a list of <see cref="PrivilegeDto"/> mapped from the repository entities.
        /// </summary>
        [Fact]
        public async Task Handle_ReturnsMappedPrivilegeDtos()
        {
            // Arrange: Create a mock repository and setup returned privileges
            var mockRepo = new Mock<IPrivilegeRepository>();
            var privileges = new List<Domain.Entity.Privilege>
            {
                new Domain.Entity.Privilege { PrivilegeId = 1, Name = "Read", Description = "Read privilege" },
                new Domain.Entity.Privilege { PrivilegeId = 2, Name = "Write", Description = "Write privilege" }
            };

            // Setup mock to return the predefined list when GetAllPrivilegesAsync is called
            mockRepo.Setup(r => r.GetAllPrivilegesAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(privileges);

            // Create handler instance with mocked repository
            var handler = new GetPrivilegesQueryHandler(mockRepo.Object);
            var query = new GetPrivilegesQuery();

            // Act: Call the Handle method
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert: Verify that the result is correctly mapped and repository method was called once
            Assert.NotNull(result); // Result should not be null
            Assert.Equal(privileges.Count, result.Count); // Result count matches original list
            Assert.Equal("Read", result[0].Name); // First item mapped correctly
            Assert.Equal("Write", result[1].Name); // Second item mapped correctly
            mockRepo.Verify(r => r.GetAllPrivilegesAsync(It.IsAny<CancellationToken>()), Times.Once); // Repository called once
        }
    }
}
