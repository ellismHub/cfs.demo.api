using Microsoft.EntityFrameworkCore;
using cfs.demo.Models;

namespace cfs.demo.Data
{
    public class CfsDbContext : DbContext
    {
        public CfsDbContext(DbContextOptions<CfsDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.FirstName).HasMaxLength(200);
                b.Property(u => u.LastName).HasMaxLength(200);
                b.Property(u => u.Age).IsRequired();
                b.Property(u => u.City).IsRequired().HasMaxLength(200);
                b.Property(u => u.State).IsRequired().HasMaxLength(200);
                b.Property(u => u.Pincode).IsRequired().HasMaxLength(10);
            });
        }
    }
}