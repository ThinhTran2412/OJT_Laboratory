using WareHouse_Service.Domain.Entity;

namespace WareHouse_Service.Application.Interface
{
    public interface IInstrumentRepository
    {
        Task<List<Instrument>> GetByTestTypeAsync(string testType);
    }
}
