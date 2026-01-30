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
                b.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
                b.Property(u => u.LastName).IsRequired().HasMaxLength(100);
                b.Property(u => u.Age).IsRequired();
                b.Property(u => u.City).IsRequired();
                b.Property(u => u.State).IsRequired();
                b.Property(u => u.Pincode).IsRequired().HasMaxLength(10);
            });
        }
    }
}