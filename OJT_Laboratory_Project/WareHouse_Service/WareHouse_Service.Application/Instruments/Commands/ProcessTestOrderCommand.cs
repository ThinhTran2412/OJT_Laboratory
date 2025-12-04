using MediatR;
using WareHouse_Service.Application.DTOs;

namespace WareHouse_Service.Application.Instruments.Commands
{
    public class ProcessTestOrderCommand : IRequest<RawTestResultDTO>
    {
        public Guid TestOrderId { get; set; }
        public string TestType { get; set; } = string.Empty;
        public ProcessTestOrderCommand(Guid testOrderId, string testType)
        {
            TestOrderId = testOrderId;
            TestType = testType;
        }   
    }
}
