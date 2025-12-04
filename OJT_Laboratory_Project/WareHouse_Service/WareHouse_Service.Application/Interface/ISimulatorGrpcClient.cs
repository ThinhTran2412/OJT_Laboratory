using WareHouse_Service.Application.DTOs;

namespace WareHouse_Service.Application.Interface
{
    public interface ISimulatorGrpcClient
    {
        Task<RawTestResultDTO?> CreateAndGetRawResultsAsync(Guid testOrderId, string testType);
    }
}
