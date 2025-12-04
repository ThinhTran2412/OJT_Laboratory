using System.Collections.Generic;
using System.Linq;
using Laboratory_Service.Application.Interface;
using Laboratory_Service.Domain.Entity;
using Laboratory_Service.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Laboratory_Service.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for TestResult operations.
    /// </summary>
    /// <seealso cref="Laboratory_Service.Application.Interface.ITestResultRepository" />
    public class TestResultRepository : ITestResultRepository
    {
        /// <summary>
        /// The database context
        /// </summary>
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestResultRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public TestResultRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Adds a single test result asynchronously.
        /// </summary>
        /// <param name="testResult">The test result to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The added test result.</returns>
        public async Task<TestResult> AddAsync(TestResult testResult, CancellationToken cancellationToken = default)
        {
            await _dbContext.TestResults.AddAsync(testResult, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return testResult;
        }

        /// <summary>
        /// Adds multiple test results asynchronously (bulk insert).
        /// Uses AddRange for better performance when inserting multiple records.
        /// </summary>
        /// <param name="testResults">The list of test results to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The number of test results added.</returns>
        public async Task<int> AddRangeAsync(List<TestResult> testResults, CancellationToken cancellationToken = default)
        {
            if (testResults == null || !testResults.Any())
            {
                return 0;
            }

            await _dbContext.TestResults.AddRangeAsync(testResults, cancellationToken);
            var count = await _dbContext.SaveChangesAsync(cancellationToken);
            return count;
        }

        /// <summary>
        /// Gets all test results by patient identifier with test order information.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Collection of test results with test order information.</returns>
        public async Task<IEnumerable<TestResult>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.TestResults
                .Include(tr => tr.TestOrder)
                    .ThenInclude(to => to!.MedicalRecord)
                        .ThenInclude(mr => mr.Patient)
                .Where(tr => tr.TestOrder != null 
                    && tr.TestOrder.MedicalRecord != null
                    && tr.TestOrder.MedicalRecord.PatientId == patientId
                    && !tr.TestOrder.IsDeleted)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets the training dataset asynchronous.
        /// Only TestResults with ValueNumeric and ResultStatus are included.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Collection of TestResults for training.</returns>
        public async Task<IEnumerable<TestResult>> GetTrainingDatasetAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.TestResults
                .Where(tr => tr.ValueNumeric.HasValue && !string.IsNullOrEmpty(tr.ResultStatus))
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets test results by TestOrderId.
        /// </summary>
        /// <param name="testOrderId">The test order identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of test results for the specified test order, ordered by TestCode.</returns>
        public async Task<List<TestResult>> GetByTestOrderIdAsync(Guid testOrderId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.TestResults
                .AsNoTracking()
                .Where(tr => tr.TestOrderId == testOrderId)
                .OrderBy(tr => tr.TestCode)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<List<TestResult>> GetByTestCodesAsync(IEnumerable<string> testCodes, CancellationToken cancellationToken = default)
        {
            var normalizedCodes = testCodes?
                .Where(code => !string.IsNullOrWhiteSpace(code))
                .Select(code => code.Trim().ToUpperInvariant())
                .Distinct()
                .ToList();

            if (normalizedCodes == null || normalizedCodes.Count == 0)
            {
                return new List<TestResult>();
            }

            return await _dbContext.TestResults
                .Include(tr => tr.TestOrder)
                    .ThenInclude(order => order.MedicalRecord)
                        .ThenInclude(record => record.Patient)
                .Where(tr =>
                    tr.TestOrder != null &&
                    !tr.TestOrder.IsDeleted &&
                    normalizedCodes.Contains(tr.TestCode.ToUpper()))
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Updates multiple TestResults asynchronously.
        /// </summary>
        /// <param name="testResults">The test results to update.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task UpdateRangeAsync(IEnumerable<TestResult> testResults, CancellationToken cancellationToken = default)
        {
            var resultsList = testResults.ToList();
            if (!resultsList.Any())
            {
                return;
            }

            // Get IDs of results to update
            var resultIds = resultsList.Select(r => r.TestResultId).ToList();
            
            // Detach any already tracked entities with same IDs to avoid conflicts
            var trackedEntities = _dbContext.TestResults.Local
                .Where(e => resultIds.Contains(e.TestResultId))
                .ToList();
            
            foreach (var tracked in trackedEntities)
            {
                _dbContext.Entry(tracked).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }
            
            // Query entities from database (without navigation properties) to avoid SQL issues
            var existingResults = await _dbContext.TestResults
                .Where(tr => resultIds.Contains(tr.TestResultId))
                .ToListAsync(cancellationToken);

            // Update properties on existing entities
            foreach (var updatedResult in resultsList)
            {
                var existing = existingResults.FirstOrDefault(e => e.TestResultId == updatedResult.TestResultId);
                if (existing != null)
                {
                    // Update only the properties that might have changed
                    existing.ResultStatus = updatedResult.ResultStatus;
                    existing.ReviewedByAI = updatedResult.ReviewedByAI;
                    existing.AiReviewedDate = updatedResult.AiReviewedDate;
                    existing.IsConfirmed = updatedResult.IsConfirmed;
                    existing.ConfirmedByUserId = updatedResult.ConfirmedByUserId;
                    existing.ConfirmedDate = updatedResult.ConfirmedDate;
                    existing.ReviewedByUserId = updatedResult.ReviewedByUserId;
                    existing.ReviewedDate = updatedResult.ReviewedDate;
                }
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateFlagsAsync(IEnumerable<TestResult> testResults, CancellationToken cancellationToken = default)
        {
            var updatedList = testResults.ToList();
            if (!updatedList.Any())
            {
                return;
            }

            var ids = updatedList.Select(r => r.TestResultId).ToList();

            var existingResults = await _dbContext.TestResults
                .Where(tr => ids.Contains(tr.TestResultId))
                .ToListAsync(cancellationToken);

            foreach (var updated in updatedList)
            {
                var entity = existingResults.FirstOrDefault(e => e.TestResultId == updated.TestResultId);
                if (entity == null)
                {
                    continue;
                }

                entity.Flag = updated.Flag;
                entity.FlaggedAt = updated.FlaggedAt;
                entity.FlaggedBy = updated.FlaggedBy;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
