using Microsoft.EntityFrameworkCore;
using WareHouse_Service.Domain.Entity;

namespace WareHouse_Service.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public static string? SchemaName { get; set; }
        
        public DbSet<Instrument> Instruments { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Đọc schema từ static property hoặc dùng default
            var schemaName = SchemaName ?? "warehouse_service";
            modelBuilder.HasDefaultSchema(schemaName);
            
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Instrument>(enity =>
            {
                enity.HasKey(e => e.Id);
            });
        }
    }
}
