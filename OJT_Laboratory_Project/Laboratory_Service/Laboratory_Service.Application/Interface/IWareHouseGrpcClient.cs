using Laboratory_Service.Application.DTOs.TestResult;

namespace Laboratory_Service.Application.Interface
{
    public interface IWareHouseGrpcClient
    {
        Task<RawTestResultDTO?> ProcessTestOrder(Guid testOrderId, string testType);
    }
}
