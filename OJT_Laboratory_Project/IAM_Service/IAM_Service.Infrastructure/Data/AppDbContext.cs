using IAM_Service.Domain.Entity;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Database context for the application, managing entity sets and database connections.
/// </summary>
namespace IAM_Service.Infrastructure.Data
{
    /// <summary>
    /// Application EF Core DbContext. Holds DbSets and relationship mapping.
    /// Represents the application's database context, including DbSet properties for each entity.
    /// </summary>
    public class AppDbContext : DbContext
    {
        // Static property để lưu schema name (set từ InfrastructureDI)
        public static string? SchemaName { get; set; }

        // --- DbSet Properties ---

        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        /// <summary>Gets or sets the DbSet for User entities.</summary>
        public DbSet<User> Users { get; set; }

        /// <summary>Security roles.</summary>
        public DbSet<Role> Roles { get; set; }

        /// <summary>Atomic privileges assigned to roles.</summary>
        public DbSet<Privilege> Privileges { get; set; }

        /// <summary>Join table for Role &lt;-&gt; Privilege (many-to-many).</summary>
        public DbSet<RolePrivilege> RolePrivileges { get; set; }
        /// <summary>Stores password reset tokens for users.</summary>
        public DbSet<PasswordReset> PasswordResets { get; set; }
        /// <summary>Join table for User &lt;-&gt; Privilege (many-to-many).</summary>
        public DbSet<UserPrivilege> UserPrivileges { get; set; }

        // --- Constructor ---

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDbContext"/> class using the specified options.
        /// </summary>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // --- Model Configuration ---

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Đọc schema từ static property hoặc dùng default
            var schemaName = SchemaName ?? "iam_service";
            modelBuilder.HasDefaultSchema(schemaName);
            
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AuditLog>()
    .Property(a => a.Id)
    .ValueGeneratedOnAdd(); // ✅ đảm bảo auto tăng ID

            // Configure composite primary key for join entity
            modelBuilder.Entity<RolePrivilege>()
                .HasKey(rp => new { rp.RoleId, rp.PrivilegeId });

            // Role 1..* RolePrivileges
            modelBuilder.Entity<RolePrivilege>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePrivileges)
                .HasForeignKey(rp => rp.RoleId);

            // Privilege 1..* RolePrivileges
            modelBuilder.Entity<RolePrivilege>()
                .HasOne(rp => rp.Privilege)
                .WithMany(p => p.RolePrivileges)
                .HasForeignKey(rp => rp.PrivilegeId);

            // User *..1 Role relationship
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.SetNull);

            // UserPrivilege composite key
            modelBuilder.Entity<UserPrivilege>()
                .HasKey(up => new { up.UserId, up.PrivilegeId });

            // User 1..* UserPrivileges
            modelBuilder.Entity<UserPrivilege>()
                .HasOne(up => up.User)
                .WithMany(u => u.UserPrivileges)
                .HasForeignKey(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Privilege 1..* UserPrivileges
            modelBuilder.Entity<UserPrivilege>()
                .HasOne(up => up.Privilege)
                .WithMany(p => p.UserPrivileges)
                .HasForeignKey(up => up.PrivilegeId)
                .OnDelete(DeleteBehavior.Cascade);


            // --- Seed Privileges (29 Privileges) ---
            modelBuilder.Entity<Privilege>().HasData(
                // Patient Test Order Privileges (1-8)
                new Privilege { PrivilegeId = 1, Name = "READ_ONLY", Description = "Only have right to view patient test orders and patient test order results." },
                new Privilege { PrivilegeId = 2, Name = "CREATE_TEST_ORDER", Description = "Have right to create a new patient test order" },
                new Privilege { PrivilegeId = 3, Name = "MODIFY_TEST_ORDER", Description = "Have right to modify information a patient test order." },
                new Privilege { PrivilegeId = 4, Name = "DELETE_TEST_ORDER", Description = "Have right to delete an exist test order." },
                new Privilege { PrivilegeId = 5, Name = "REVIEW_TEST_ORDER", Description = "Have right to review, modify test result of test order" },
                new Privilege { PrivilegeId = 6, Name = "ADD_COMMENT", Description = "Have right to add a new comment for test result" },
                new Privilege { PrivilegeId = 7, Name = "MODIFY_COMMENT", Description = "Have right to modify a comment." },
                new Privilege { PrivilegeId = 8, Name = "DELETE_COMMENT", Description = "Have right to delete a comment." },
                // Configuration Privileges (9-12)
                new Privilege { PrivilegeId = 9, Name = "VIEW_CONFIGURATION", Description = "Have right to view, add, modify and delete configurations." },
                new Privilege { PrivilegeId = 10, Name = "CREATE_CONFIGURATION", Description = "Have right to add a new configuration." },
                new Privilege { PrivilegeId = 11, Name = "MODIFY_CONFIGURATION", Description = "Have right to modify a configuration." },
                new Privilege { PrivilegeId = 12, Name = "DELETE_CONFIGURATION", Description = "Have right to delete a configuration." },
                // User Management Privileges (13-17)
                new Privilege { PrivilegeId = 13, Name = "VIEW_USER", Description = "Have right to view all user profiles" },
                new Privilege { PrivilegeId = 14, Name = "CREATE_USER", Description = "Have right to create a new user." },
                new Privilege { PrivilegeId = 15, Name = "MODIFY_USER", Description = "Have right to modify a user." },
                new Privilege { PrivilegeId = 16, Name = "DELETE_USER", Description = "Have right to delete a user." },
                new Privilege { PrivilegeId = 17, Name = "LOCK_UNLOCK_USER", Description = "Have right to lock or unlock a user." },
                // Role Management Privileges (18-21)
                new Privilege { PrivilegeId = 18, Name = "VIEW_ROLE", Description = "Have right to view all role privileges." },
                new Privilege { PrivilegeId = 19, Name = "CREATE_ROLE", Description = "Have right to create a new custom role." },
                new Privilege { PrivilegeId = 20, Name = "UPDATE_ROLE", Description = "Have right to modify privileges of custom role." },
                new Privilege { PrivilegeId = 21, Name = "DELETE_ROLE", Description = "Have right to delete a custom role." },
                // Lab Management Privileges (22-29)
                new Privilege { PrivilegeId = 22, Name = "VIEW_EVENT_LOGS", Description = "Have right to view event logs" },
                new Privilege { PrivilegeId = 23, Name = "ADD_REAGENTS", Description = "Have right to add new reagents." },
                new Privilege { PrivilegeId = 24, Name = "MODIFY_REAGENTS", Description = "Have right to modify reagent information." },
                new Privilege { PrivilegeId = 25, Name = "DELETE_REAGENTS", Description = "Have right to delete a regents" },
                new Privilege { PrivilegeId = 26, Name = "ADD_INSTRUMENT", Description = "Have right to add a new instrument into system management" },
                new Privilege { PrivilegeId = 27, Name = "VIEW_INSTRUMENT", Description = "Have right to view all instrument and check instrument status." },
                new Privilege { PrivilegeId = 28, Name = "ACTIVATE_DEACTIVATE_INSTRUMENT", Description = "Have right to activate or deactivate instrument" },
                new Privilege { PrivilegeId = 29, Name = "EXECUTE_BLOOD_TESTING", Description = "Have right to execute a blood testing" }
            );

            // --- Seed Roles (5 Roles) ---
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, Name = "ADMIN", Description = "Full administrative access." },
                new Role { RoleId = 2, Name = "LAB_MANAGER", Description = "Manager role for the lab." },
                new Role { RoleId = 3, Name = "LAB_USER", Description = "Standard lab user." },
                new Role { RoleId = 4, Name = "SERVICE", Description = "Service/Maintenance role." },
                new Role { RoleId = 5, Name = "CUSTOM_ROLE_DEFAULT", Description = "Default minimal access role." }
            );

            // --- Seed RolePrivileges (Mapping) ---
            var rolePrivileges = new List<RolePrivilege>();

            // 1. ADMIN (RoleId: 1) - Có quyền tất cả (1-29)
            for (int pId = 1; pId <= 29; pId++)
            {
                rolePrivileges.Add(new RolePrivilege { RoleId = 1, PrivilegeId = pId });
            }

            // 2. LAB_MANAGER (RoleId: 2) - Quyền: 
            var labManagerPrivilegeIds = new int[] { 1, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 25, 26, 27, 28 };
            foreach (int pId in labManagerPrivilegeIds)
            {
                rolePrivileges.Add(new RolePrivilege { RoleId = 2, PrivilegeId = pId });
            }

            // 3. LAB_USER (RoleId: 3) - Quyền:
            var labUserPrivilegeIds = new int[] { 2, 3, 4, 5, 6, 7, 8, 22, 23, 24, 25, 26, 27, 28, 29 };
            foreach (int pId in labUserPrivilegeIds)
            {
                rolePrivileges.Add(new RolePrivilege { RoleId = 3, PrivilegeId = pId });
            }

            // 4. SERVICE (RoleId: 4) - Quyền: 
            var servicePrivilegeIds = new int[] { 9, 10, 11, 12, 22,23, 24, 25, 26, 27, 28, 29 };
            foreach (int pId in servicePrivilegeIds)
            {
                rolePrivileges.Add(new RolePrivilege { RoleId = 4, PrivilegeId = pId });
            }

            // 5. CUSTOM_ROLE_DEFAULT (RoleId: 5) - Quyền: 1
            var customRolePrivilegeIds = new int[] { 1};
            foreach (int pId in customRolePrivilegeIds)
            {
                rolePrivileges.Add(new RolePrivilege { RoleId = 5, PrivilegeId = pId });
            }

            // Áp dụng dữ liệu seed cho bảng join
            modelBuilder.Entity<RolePrivilege>().HasData(rolePrivileges);

            // --- RefreshToken configuration ---
            modelBuilder.Entity<RefreshToken>(builder =>
            {
                builder.HasKey(r => r.Id);

                builder.Property(r => r.Token)
                    .IsRequired()
                    .HasMaxLength(200);

                builder.HasIndex(r => r.Token)
                    .IsUnique();

                builder.Property(r => r.ExpiryDate)
                    .IsRequired();

                builder.Property(r => r.IsRevoked)
                    .HasDefaultValue(false);

                builder.HasOne(r => r.User)
                    .WithMany(u => u.RefreshTokens)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}