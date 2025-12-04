using Laboratory_Service.Application.DTOs.TestResult;
using MediatR;

namespace Laboratory_Service.Application.Test_Result
{
    public class ProcessTestResultMessageCommand : IRequest<TestResultIngressResponseDto>
    {
        /// <summary>
        /// Test Order ID to link results to existing test order.
        /// </summary>
        public Guid TestOrderId { get; set; }
        public string TestType { get; set; } = string.Empty;

    }
}
