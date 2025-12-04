using Microsoft.EntityFrameworkCore;
using Simulator.Application.DTOs;
using Simulator.Application.Interface;
using Simulator.Domain.Entity;
using Simulator.Infastructure.Data;

namespace Simulator.Infastructure.Repository
{
    /// <summary>
    /// Implements the raw test result repository
    /// </summary>
    /// <seealso cref="Simulator.Application.Interface.IRawTestResultRepository" />
    public class RawTestResultRepository : IRawTestResultRepository
    {
        /// <summary>
        /// The database context
        /// </summary>
        private readonly AppDbContext _dbContext;
        /// <summary>
        /// Initializes a new instance of the <see cref="RawTestResultRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public RawTestResultRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        /// <summary>
        /// Adds the range asynchronous.
        /// </summary>
        /// <param name="results">The results.</param>
        public async Task AddRangeAsync(List<RawTestResultDTO> results)
        {
            if (results == null || !results.Any())
            {
                return;
            }

            var entitiesToInsert = new List<RawTestResult>();

            foreach (var rawResultDto in results)
            {
                foreach (var itemDto in rawResultDto.Results)
                {
                    var entity = new RawTestResult
                    {
                        TestOrderId = rawResultDto.TestOrderId,
                        Instrument = rawResultDto.Instrument,
                        PerformedDate = rawResultDto.PerformedDate,

                        TestCode = itemDto.TestCode,
                        Parameter = itemDto.Parameter,
                        ValueNumeric = itemDto.ValueNumeric,
                        ValueText = itemDto.ValueText,
                        Unit = itemDto.Unit,
                        ReferenceRange = itemDto.ReferenceRange,
                        Status = itemDto.Status,
                        CreatedAt = DateTime.UtcNow
                    };

                    entitiesToInsert.Add(entity);
                }
            }

            if (entitiesToInsert.Any())
            {
                await _dbContext.RawTestResults.AddRangeAsync(entitiesToInsert);
                await _dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Gets the results by order identifier asynchronous.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns></returns>
        public async Task<List<RawTestResult>> GetResultsByOrderIdAsync(Guid orderId)
        {
            return await _dbContext.RawTestResults
                                 .Where(r => r.TestOrderId == orderId)
                                 .ToListAsync();
        }
        /// <summary>
        /// Gets the unsent results asynchronous.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<RawTestResultDTO>> GetUnsentResultsAsync()
        {

            var unsentEntities = await _dbContext.RawTestResults
                .Where(r => !r.IsSent)
                .ToListAsync();

            if (!unsentEntities.Any())
            {
                return new List<RawTestResultDTO>();
            }

            var groupedResults = unsentEntities
                .GroupBy(r => new { r.TestOrderId, r.Instrument, r.PerformedDate })
                .Select(g => new RawTestResultDTO
                {
                    TestOrderId = g.Key.TestOrderId,
                    Instrument = g.Key.Instrument,
                    PerformedDate = g.Key.PerformedDate,
                    Results = g.Select(e => new RawResultItemDTO
                    {
                        TestCode = e.TestCode,
                        Parameter = e.Parameter,
                        ValueNumeric = e.ValueNumeric,
                        ValueText = e.ValueText,
                        Unit = e.Unit,
                        ReferenceRange = e.ReferenceRange,
                        Status = e.Status
                    }).ToList()
                })
                .ToList();

            return groupedResults;
        }

        /// <summary>
        /// Marks as sent asynchronous.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        public async Task MarkAsSentAsync(Guid orderId)
        {
            var entities = await _dbContext.RawTestResults
                                           .Where(r => r.TestOrderId == orderId)
                                           .ToListAsync();

            if (entities.Any())
            {
                foreach (var entity in entities)
                {
                    entity.IsSent = true;
                }

                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
   
