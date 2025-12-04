using System.Text.Json;
using IAM_Service.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace IAM_Service.Infrastructure.Interceptor
{
    /// <summary>
    /// Override the savechange function to save the changes to the log.
    /// </summary>
    /// <seealso cref="Microsoft.EntityFrameworkCore.Diagnostics.SaveChangesInterceptor" />
    public class AuditLogInterceptor : SaveChangesInterceptor
    {
        /// <summary>
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="result"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <inheritdoc />
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;
            if (context == null)
                return base.SavingChangesAsync(eventData, result, cancellationToken);

            var auditLogs = new List<AuditLog>();

            foreach (var entry in context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added
                         || e.State == EntityState.Modified
                         || e.State == EntityState.Deleted))
            {
                var entityName = entry.Entity.GetType().Name;
                var userEmail = "system";
                var action = entry.State.ToString().ToUpper();

                var changes = GetChanges(entry);

                auditLogs.Add(new AuditLog
                {
                    UserEmail = userEmail,
                    EntityName = entityName,
                    Action = action,
                    Timestamp = DateTime.UtcNow,
                    Changes = JsonSerializer.Serialize(changes, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    })
                });
            }

            if (auditLogs.Any())
                context.AddRange(auditLogs);

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }


        /// <summary>
        /// Gets the changes.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns></returns>
        private static object GetChanges(EntityEntry entry)
        {
            var changes = new Dictionary<string, object?>();

            if (entry.State == EntityState.Modified)
            {
                foreach (var prop in entry.Properties)
                {
                    if (prop.IsModified)
                    {
                        changes[prop.Metadata.Name] = new
                        {
                            OldValue = prop.OriginalValue,
                            NewValue = prop.CurrentValue
                        };
                    }
                }
            }
            else if (entry.State == EntityState.Added)
            {
                foreach (var prop in entry.Properties)
                {
                    changes[prop.Metadata.Name] = new
                    {
                        OldValue = (object?)null,
                        NewValue = prop.CurrentValue
                    };
                }
            }
            else if (entry.State == EntityState.Deleted)
            {
                foreach (var prop in entry.Properties)
                {
                    changes[prop.Metadata.Name] = new
                    {
                        OldValue = prop.OriginalValue,
                        NewValue = (object?)null
                    };
                }
            }

            return changes;
        }
    }
}
