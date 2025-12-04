using Laboratory_Service.Application.DTOs.Pagination;
using Laboratory_Service.Application.Interface;
using Laboratory_Service.Domain.Entity;
using Laboratory_Service.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Laboratory_Service.Infrastructure.Repositories
{
    /// <summary>
    /// Implement method form ITestOrderRepository
    /// </summary>
    /// <seealso cref="Laboratory_Service.Application.Interface.ITestOrderRepository" />
    public class TestOrderRepository : ITestOrderRepository
    {
        /// <summary>
        /// The database context
        /// </summary>
        private readonly AppDbContext _dbContext;
        /// <summary>
        /// Initializes a new instance of the <see cref="TestOrderRepository" /> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public TestOrderRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Adds the asynchronous.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task AddAsync(TestOrder order, CancellationToken cancellationToken = default)
        {
            await _dbContext.TestOrders.AddAsync(order, cancellationToken);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="testOrder">The test order.</param>
        public async Task DeleteAsync(TestOrder testOrder)
        {
            _dbContext.TestOrders.Remove(testOrder);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Gets all by patient identifier asynchronous.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<IEnumerable<TestOrder>> GetAllByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.TestOrders
                .Include(to => to.MedicalRecord)
                    .ThenInclude(mr => mr.Patient)
                .Include(to => to.TestResults)
                .Where(to => to.MedicalRecord.PatientId == patientId && !to.IsDeleted)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets the by identifier asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<TestOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.TestOrders
                .Include(o => o.MedicalRecord)
                    .ThenInclude(mr => mr.Patient)
                .Include(o => o.TestResults)
                .FirstOrDefaultAsync(o => o.TestOrderId == id, cancellationToken);
        }

        /// <summary>
        /// Gets the by identifier for update (without includes to avoid SQL issues).
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<TestOrder?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.TestOrders
                .FirstOrDefaultAsync(o => o.TestOrderId == id, cancellationToken);
        }

        /// <summary>
        /// Gets the AI review enabled status for a test order by ID (lightweight query without includes).
        /// </summary>
        /// <param name="id">The test order identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>True if AI review is enabled, false otherwise. Returns null if test order not found.</returns>
        public async Task<bool?> GetAiReviewEnabledByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.TestOrders
                .Where(o => o.TestOrderId == id)
                .Select(o => o.IsAiReviewEnabled)
                .FirstOrDefaultAsync(cancellationToken);
        }


        /// <summary>
        /// Saves the changes asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="testOrder">The test order.</param>
        public async Task UpdateAsync(TestOrder testOrder)
        {
            // Detach navigation properties to avoid SQL issues when updating
            // EF Core will try to track navigation properties which can cause SQL errors
            var testResults = testOrder.TestResults;
            var medicalRecord = testOrder.MedicalRecord;
            
            // Clear navigation properties temporarily
            testOrder.TestResults = null;
            testOrder.MedicalRecord = null;
            
            // Check if entity is already tracked
            var existingEntity = _dbContext.TestOrders.Local.FirstOrDefault(e => e.TestOrderId == testOrder.TestOrderId);
            if (existingEntity != null)
            {
                // Update properties on tracked entity
                _dbContext.Entry(existingEntity).CurrentValues.SetValues(testOrder);
            }
            else
            {
                // Attach and update
                _dbContext.TestOrders.Update(testOrder);
            }
            
            await _dbContext.SaveChangesAsync();
            
            // Restore navigation properties (for return value, not persisted)
            testOrder.TestResults = testResults;
            testOrder.MedicalRecord = medicalRecord;
        }

        /// <summary>
        /// Updates only the status of a test order (lightweight update without navigation properties).
        /// </summary>
        /// <param name="testOrderId">The test order identifier.</param>
        /// <param name="status">The new status.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task UpdateStatusAsync(Guid testOrderId, string status, CancellationToken cancellationToken = default)
        {
            var testOrder = await _dbContext.TestOrders
                .FirstOrDefaultAsync(o => o.TestOrderId == testOrderId, cancellationToken);
            
            if (testOrder != null)
            {
                testOrder.Status = status;
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }
        /// <summary>
        /// Query patient test orders with search, paging, sorting and status filtering.
        /// Returns a paged result for handlers.
        /// </summary>
        /// <param name="search">Search keyword.</param>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="sortBy">Sort column.</param>
        /// <param name="sortDesc">Sort direction.</param>
        /// <param name="status">Status filter.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        /// Paged result of test orders.
        /// </returns>
        public async Task<PagedResult<TestOrder>> GetTestOrdersAsync(
            string? search,
            int page,
            int pageSize,
            string? sortBy,
            bool sortDesc,
            string? status,
            CancellationToken cancellationToken)
        {
            // Base query with eager loading for related medical record and patient. No tracking for read performance.
            IQueryable<TestOrder> query = _dbContext.TestOrders
                .Include(to => to.MedicalRecord)
                    .ThenInclude(mr => mr.Patient)
                .Where(to => !to.IsDeleted)
                .AsNoTracking();

            // Apply status filter if provided
            if (!string.IsNullOrWhiteSpace(status))
            {
                string statusFilter = status.Trim();
                query = query.Where(to => to.Status == statusFilter);
            }

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(search))
            {
                string s = search.Trim().ToLower();
                query = query.Where(to =>
                    to.MedicalRecord.Patient.FullName.ToLower().Contains(s)
                    || to.MedicalRecord.Patient.PhoneNumber.ToLower().Contains(s)
                    || to.Status.ToLower().Contains(s)
                    || to.OrderCode.ToLower().Contains(s)
                );
            }

            // Apply sorting by requested column (default to createdDate descending for most recent)
            if (string.IsNullOrEmpty(sortBy))
            {
                sortBy = "createdDate";
                sortDesc = true; // Default to descending (most recent first)
            }
            string sort = sortBy.ToLower();

            switch (sort)
            {
                case "id":
                case "testorderid":
                    query = sortDesc ? query.OrderByDescending(to => to.TestOrderId) : query.OrderBy(to => to.TestOrderId);
                    break;
                case "patientname":
                case "patient":
                    query = sortDesc ? query.OrderByDescending(to => to.MedicalRecord.Patient.FullName)
                                     : query.OrderBy(to => to.MedicalRecord.Patient.FullName);
                    break;
                case "age":
                    query = sortDesc ? query.OrderByDescending(to => to.MedicalRecord.Patient.Age)
                                     : query.OrderBy(to => to.MedicalRecord.Patient.Age);
                    break;
                case "gender":
                    query = sortDesc ? query.OrderByDescending(to => to.MedicalRecord.Patient.Gender)
                                     : query.OrderBy(to => to.MedicalRecord.Patient.Gender);
                    break;
                case "phonenumber":
                case "phone":
                    query = sortDesc ? query.OrderByDescending(to => to.MedicalRecord.Patient.PhoneNumber)
                                     : query.OrderBy(to => to.MedicalRecord.Patient.PhoneNumber);
                    break;
                case "status":
                    query = sortDesc ? query.OrderByDescending(to => to.Status) : query.OrderBy(to => to.Status);
                    break;
                case "rundate":
                case "run":
                    query = sortDesc ? query.OrderByDescending(to => to.RunDate) : query.OrderBy(to => to.RunDate);
                    break;
                case "createddate":
                case "created":
                default:
                    query = sortDesc ? query.OrderByDescending(to => to.CreatedAt) : query.OrderBy(to => to.CreatedAt);
                    break;
            }

            // Count BEFORE paging to get total records
            int total = await query.CountAsync(cancellationToken);

            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            // Apply paging at the database level
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<TestOrder>
            {
                Items = items,
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }
        /// <summary>
        /// Gets the by identifier with results asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<TestOrder?> GetByIdWithResultsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.TestOrders
                .Include(o => o.TestResults)      
                .Include(o => o.MedicalRecord)
                .ThenInclude(mr => mr.Patient)
                .FirstOrDefaultAsync(o => o.TestOrderId == id, cancellationToken);
        }

        /// <summary>
        /// Gets the by identifier with results only (for AI review operations, without MedicalRecord to avoid SQL issues).
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<TestOrder?> GetByIdWithResultsOnlyAsync(Guid id, CancellationToken cancellationToken = default)
        {
            // Only include TestResults to avoid SQL issues with MedicalRecord/Patient includes
            // Use AsNoTracking to prevent EF Core from tracking navigation properties
            var testOrder = await _dbContext.TestOrders
                .Include(o => o.TestResults)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.TestOrderId == id, cancellationToken);
            
            if (testOrder != null)
            {
                // Detach TestOrder navigation from TestResults to avoid tracking issues
                foreach (var result in testOrder.TestResults)
                {
                    result.TestOrder = null;
                }
            }
            
            return testOrder;
        }

    }
}
