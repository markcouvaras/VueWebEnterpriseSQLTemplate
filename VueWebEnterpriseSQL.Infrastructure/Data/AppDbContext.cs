using Microsoft.EntityFrameworkCore;
using VueWebEnterpriseSQL.Domain.Entities; // Ensure you create this namespace

namespace VueWebEnterpriseSQL.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Define your tables here
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Optional: Configure default schema or constraints
            // modelBuilder.HasDefaultSchema("public");
        }
    }
}