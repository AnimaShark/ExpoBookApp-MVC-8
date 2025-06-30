using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ExpoBookApp.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<PasswordHistory> PasswordHistories { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<Venues> Venues { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            {
                modelBuilder.Entity<Venues>()
                    .Property(v => v.ApprovalStatus)
                    .HasConversion<string>();

                base.OnModelCreating(modelBuilder);
            }
        }
    }
}

