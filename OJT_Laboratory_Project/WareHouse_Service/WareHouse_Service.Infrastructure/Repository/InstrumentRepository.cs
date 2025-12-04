using WareHouse_Service.Application.Interface;
using WareHouse_Service.Domain.Entity;
using WareHouse_Service.Infrastructure.Data;

namespace WareHouse_Service.Infrastructure.Repository
{
    public class InstrumentRepository : IInstrumentRepository
    {
        private readonly AppDbContext _DbContext;
        public InstrumentRepository(AppDbContext dbContext)
        {
            _DbContext = dbContext;
        }
        public async Task<List<Instrument>> GetByTestTypeAsync(string testType)
        {
            return await Task.FromResult(_DbContext.Instruments
                .Where(i => i.SupportTestType.Contains(testType) && i.Status.Contains("Pending"))
                .ToList());
        }
    }
}
