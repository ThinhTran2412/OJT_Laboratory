using Laboratory_Service.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Laboratory_Service.Infrastructure.Data
{
    /// <summary>
    /// create appDbcontext
    /// </summary>
    /// <seealso cref="Microsoft.EntityFrameworkCore.DbContext" />
    public class AppDbContext : DbContext
    {
        // Static property để lưu schema name (set từ InfrastructureDI)
        public static string? SchemaName { get; set; }

        // --- DbSet Properties ---
        /// <summary>
        /// Gets or sets the patients.
        /// </summary>
        /// <value>
        /// The patients.
        /// </value>
        public DbSet<Patient> Patients { get; set; } = null!;
        /// <summary>
        /// Gets or sets the medical records.
        /// </summary>
        /// <value>
        /// The medical records.
        /// </value>
        public DbSet<MedicalRecord> MedicalRecords { get; set; } = null!;
        /// <summary>
        /// Gets or sets the test orders.
        /// </summary>
        /// <value>
        /// The test orders.
        /// </value>
        public DbSet<TestOrder> TestOrders { get; set; } = null!;
        /// <summary>
        /// Gets or sets the test results.
        /// </summary>
        /// <value>
        /// The test results.
        /// </value>
        public DbSet<TestResult> TestResults { get; set; } = null!;
        /// <summary>
        /// Gets or sets the flagging configurations.
        /// </summary>
        /// <value>
        /// The flagging configurations.
        /// </value>
        public DbSet<FlaggingConfig> FlaggingConfigs { get; set; } = null!;
        /// <summary>
        /// Gets or sets the event logs.
        /// </summary>
        /// <value>
        /// The event logs.
        /// </value>
        public DbSet<EventLog> EventLogs { get; set; } = null!;
        public DbSet<RawBackup> RawBackups { get; set; } = null!;
        /// <summary>
        /// Gets or sets the processed messages.
        /// </summary>
        /// <value>
        /// The processed messages.
        /// </value>
        public DbSet<ProcessedMessage> ProcessedMessages { get; set; } = null!;
        /// <summary>
        /// Gets or sets the medical record histories.
        /// </summary>
        /// <value>
        /// The medical record histories.
        /// </value>
        public DbSet<MedicalRecordHistory> MedicalRecordHistories { get; set; } = null!;
        public DbSet<Comment> Comment { get; set; } = null!;

        // --- Constructor ---
        /// <summary>
        /// Initializes a new instance of the <see cref="AppDbContext"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // --- Model Configuration ---
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
            var schemaName = SchemaName ?? "laboratory_service";
            modelBuilder.HasDefaultSchema(schemaName);
            
            base.OnModelCreating(modelBuilder);

            // -------------------- Patient configuration --------------------
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasKey(e => e.PatientId);

                entity.Property(e => e.IdentifyNumber).IsRequired();
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Gender).IsRequired().HasMaxLength(10);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);

                entity.HasIndex(e => e.IdentifyNumber).IsUnique();
                entity.HasIndex(e => e.Email);
                entity.HasIndex(e => e.PhoneNumber);

                // 1 Patient -> 1 MedicalRecord
                entity.HasOne(e => e.MedicalRecord)
                      .WithOne(m => m.Patient)
                      .HasForeignKey<MedicalRecord>(m => m.PatientId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // -------------------- MedicalRecord configuration --------------------
            modelBuilder.Entity<MedicalRecord>(entity =>
            {
                entity.HasKey(e => e.MedicalRecordId);
                entity.Property(e => e.CreatedBy).HasMaxLength(100);
                entity.Property(e => e.UpdatedBy).HasMaxLength(100);

                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.CreatedAt);

                // 1 MedicalRecord -> N TestOrders
                entity.HasMany(m => m.TestOrders)
                      .WithOne(to => to.MedicalRecord)
                      .HasForeignKey(to => to.MedicalRecordId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // -------------------- TestOrder configuration --------------------
            modelBuilder.Entity<TestOrder>(entity =>
            {
                entity.HasKey(e => e.TestOrderId);

                entity.Property(e => e.OrderCode).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Priority).HasMaxLength(20);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Note).HasMaxLength(1000);
                entity.Property(e => e.TestType).HasMaxLength(100);

                entity.Property(e => e.CreatedBy).HasMaxLength(100);
                entity.Property(e => e.UpdatedBy).HasMaxLength(100);
                entity.Property(e => e.DeletedBy).HasMaxLength(100);
                entity.HasIndex(e => e.OrderCode).IsUnique();
                entity.HasIndex(e => e.MedicalRecordId);
                entity.HasIndex(e => e.Status);

                // 1 TestOrder -> N TestResults
                entity.HasMany(to => to.TestResults)
                      .WithOne(tr => tr.TestOrder)
                      .HasForeignKey(tr => tr.TestOrderId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // -------------------- TestResult configuration --------------------
            modelBuilder.Entity<TestResult>(entity =>
            {
                entity.HasKey(e => e.TestResultId);


                entity.Property(e => e.TestCode).HasMaxLength(100);
                entity.Property(e => e.Parameter).HasMaxLength(200);
                entity.Property(e => e.Unit).HasMaxLength(50);
                entity.Property(e => e.ReferenceRange).HasMaxLength(200);
                entity.Property(e => e.Instrument).HasMaxLength(100);
                entity.Property(e => e.ResultStatus).HasMaxLength(50);

                entity.HasIndex(e => e.TestOrderId);
            });

            // -------------------- FlaggingConfig configuration --------------------
            modelBuilder.Entity<FlaggingConfig>(entity =>
            {
                entity.HasKey(e => e.FlaggingConfigId);

                entity.Property(e => e.TestCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ParameterName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Description)
                    .HasMaxLength(500);

                entity.Property(e => e.Unit)
                    .HasMaxLength(50);

                entity.Property(e => e.Gender)
                    .HasMaxLength(10);

                entity.Property(e => e.Min)
                    .IsRequired();

                entity.Property(e => e.Max)
                    .IsRequired();

                // Indexes
                entity.HasIndex(e => e.TestCode);
                entity.HasIndex(e => e.Gender);
                entity.HasIndex(e => e.IsActive);

                // Unique constraint: TestCode + Gender (tránh duplicate config)
                entity.HasIndex(e => new { e.TestCode, e.Gender })
                    .IsUnique();
            });

            // -------------------- Comment configuration --------------------
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(x => x.CommentId);

                entity.Property(x => x.Message)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(x => x.CreatedDate)
                    .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

                entity.HasOne<TestOrder>()
                      .WithMany()
                      .HasForeignKey(x => x.TestOrderId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<TestResult>()
                      .WithMany()
                      .HasForeignKey(x => x.TestResultId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // -------------------- EventLog configuration --------------------
            modelBuilder.Entity<EventLog>(entity =>
            {
                entity.HasKey(e => e.EventLogId);

                entity.Property(e => e.EventId).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
                entity.Property(e => e.EventLogMessage).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.OperatorName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.EntityType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CreatedOn).IsRequired();

                entity.HasIndex(e => e.CreatedOn);
            });

            // -------------------- ProcessedMessage configuration --------------------
            modelBuilder.Entity<ProcessedMessage>(entity =>
            {
                entity.HasKey(e => e.ProcessedMessageId);

                entity.Property(e => e.MessageId)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.SourceSystem)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.TestOrderId)
                    .IsRequired();

                entity.Property(e => e.ProcessedAt)
                    .IsRequired();

                entity.Property(e => e.CreatedCount)
                    .IsRequired();

                // Unique constraint on MessageId để tránh duplicate
                entity.HasIndex(e => e.MessageId)
                    .IsUnique();

                // Index for querying by TestOrderId
                entity.HasIndex(e => e.TestOrderId);

                // Index for querying by ProcessedAt
                entity.HasIndex(e => e.ProcessedAt);
            });

            // -------------------- MedicalRecordHistory configuration --------------------
            modelBuilder.Entity<MedicalRecordHistory>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.SnapshotJson).IsRequired();
                entity.Property(e => e.ChangeSummary).IsRequired();
                entity.Property(e => e.ChangedBy).IsRequired();
                entity.Property(e => e.ChangedAt).IsRequired();

                entity.HasIndex(e => e.MedicalRecordId);
                entity.HasIndex(e => e.ChangedAt);
            });
        }
    }
}
