using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ExpoBookApp.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<PasswordHistory> PasswordHistories { get; set; }
    }
}
