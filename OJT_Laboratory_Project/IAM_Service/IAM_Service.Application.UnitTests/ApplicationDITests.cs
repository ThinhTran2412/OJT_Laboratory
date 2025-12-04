using Microsoft.Extensions.DependencyInjection;
using IAM_Service.Application;

namespace IAM_Service.Application.UnitTests
{
    /// <summary>
    /// Use case test for ApplicationDI.
    /// </summary>
    public class ApplicationDITests
    {
        private readonly ServiceCollection _services;

        public ApplicationDITests()
        {
            _services = new ServiceCollection();
        }

        [Fact]
        public void Should_Register_All_Services_Without_Error()
        {
            // Act
            var result = _services.AddApplication();

            // Assert
            Assert.NotNull(result);
            Assert.Contains(result, s => s.ServiceType.Name.Contains("IMediator"));
            Assert.Contains(result, s => s.ServiceType.Name.Contains("IMapper"));
        }
    }
}
