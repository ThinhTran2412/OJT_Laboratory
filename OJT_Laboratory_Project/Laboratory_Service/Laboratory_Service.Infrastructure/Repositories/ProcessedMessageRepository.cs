using Laboratory_Service.Application.Interface;
using Laboratory_Service.Domain.Entity;
using Laboratory_Service.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Laboratory_Service.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for ProcessedMessage operations.
    /// Handles idempotency checking to prevent duplicate message processing.
    /// </summary>
    public class ProcessedMessageRepository : IProcessedMessageRepository
    {
        private readonly AppDbContext _dbContext;

        public ProcessedMessageRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Checks if a message with the given MessageId has already been processed.
        /// </summary>
        public async Task<ProcessedMessage?> GetByMessageIdAsync(string messageId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProcessedMessages
                .AsNoTracking()
                .FirstOrDefaultAsync(pm => pm.MessageId == messageId, cancellationToken);
        }

        /// <summary>
        /// Adds a new processed message record.
        /// </summary>
        public async Task<ProcessedMessage> AddAsync(ProcessedMessage processedMessage, CancellationToken cancellationToken = default)
        {
            await _dbContext.ProcessedMessages.AddAsync(processedMessage, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return processedMessage;
        }

        /// <summary>
        /// Checks if a message exists and adds it atomically if it doesn't.
        /// Uses database transaction and unique constraint to prevent race conditions.
        /// 
        /// Logic:
        /// 1. Start transaction
        /// 2. Check if message exists (with lock)
        /// 3. If not exists → Insert
        /// 4. Commit transaction
        /// 
        /// If two requests with same MessageId arrive simultaneously:
        /// - Request 1: Check → Not found → Insert → Commit
        /// - Request 2: Check → Found (locked by Request 1) → Wait → Found → Return false
        /// </summary>
        public async Task<bool> TryAddIfNotExistsAsync(string messageId, string sourceSystem, Guid testOrderId, CancellationToken cancellationToken = default)
        {
            // Sử dụng transaction để đảm bảo atomicity
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // Get schema name from AppDbContext static property
                var schemaName = AppDbContext.SchemaName ?? "laboratory_service";
                
                // Check với lock (FOR UPDATE) để tránh race condition
                // Use schema-qualified table name
                var existing = await _dbContext.ProcessedMessages
                    .FromSqlRaw(
                        $"SELECT * FROM \"{schemaName}\".\"ProcessedMessages\" WHERE \"MessageId\" = {{0}} FOR UPDATE",
                        messageId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (existing != null)
                {
                    // Message đã tồn tại → không insert
                    await transaction.RollbackAsync(cancellationToken);
                    return false;
                }

                // Message chưa tồn tại → Insert
                var processedMessage = new ProcessedMessage
                {
                    MessageId = messageId,
                    SourceSystem = sourceSystem,
                    TestOrderId = testOrderId,
                    ProcessedAt = DateTime.UtcNow,
                    CreatedCount = 0 // Sẽ được update sau khi insert test results
                };

                await _dbContext.ProcessedMessages.AddAsync(processedMessage, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return true; // Message mới được thêm
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate") == true || 
                                                ex.InnerException?.Message.Contains("unique") == true)
            {
                // Unique constraint violation → message đã tồn tại (race condition đã được handle bởi DB)
                await transaction.RollbackAsync(cancellationToken);
                return false;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        /// <summary>
        /// Updates an existing processed message.
        /// </summary>
        public async Task<ProcessedMessage> UpdateAsync(ProcessedMessage processedMessage, CancellationToken cancellationToken = default)
        {
            // Query lại với tracking để đảm bảo entity được track
            var trackedEntity = await _dbContext.ProcessedMessages
                .FirstOrDefaultAsync(pm => pm.MessageId == processedMessage.MessageId, cancellationToken);

            if (trackedEntity != null)
            {
                trackedEntity.CreatedCount = processedMessage.CreatedCount;
                await _dbContext.SaveChangesAsync(cancellationToken);
                return trackedEntity;
            }

            // Nếu không tìm thấy, update trực tiếp
            _dbContext.ProcessedMessages.Update(processedMessage);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return processedMessage;
        }
    }
}

