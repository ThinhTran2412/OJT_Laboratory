using Microsoft.EntityFrameworkCore;
using Simulator.Domain.Entity;

namespace Simulator.Infastructure.Data
{
    /// <summary>
    /// Creates the application database context
    /// </summary>
    /// <seealso cref="Microsoft.EntityFrameworkCore.DbContext" />
    public class AppDbContext : DbContext
    {
        // Static property để lưu schema name (set từ Program.cs)
        public static string? SchemaName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDbContext"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        /// <summary>
        /// Gets or sets the raw test results.
        /// </summary>
        /// <value>
        /// The raw test results.
        /// </value>
        public DbSet<RawTestResult> RawTestResults { get; set; }
        /// <summary>
        /// Override this method to further configure the model that was discovered by convention from the entity types
        /// exposed in <see cref="T:Microsoft.EntityFrameworkCore.DbSet`1" /> properties on your derived context. The resulting model may be cached
        /// and re-used for subsequent instances of your derived context.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context. Databases (and other extensions) typically
        /// define extension methods on this object that allow you to configure aspects of the model that are specific
        /// to a given database.</param>
        /// <remarks>
        /// <para>
        /// If a model is explicitly set on the options for this context (via <see cref="M:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel)" />)
        /// then this method will not be run. However, it will still run when creating a compiled model.
        /// </para>
        /// <para>
        /// See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see> for more information and
        /// examples.
        /// </para>
        /// </remarks>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Đọc schema từ static property hoặc dùng default
            var schemaName = SchemaName ?? "simulator_service";
            modelBuilder.HasDefaultSchema(schemaName);
            
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<RawTestResult>()
                .HasKey(e => e.TestResultId);
        }
    }
}
