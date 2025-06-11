using ExpoBookApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ExpoBookApp.Services
{
    public class PasswordService
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public PasswordService(AppDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }

        public string HashPassword(User user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        public bool VerifyPassword(User user, string providedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, providedPassword);
            return result == PasswordVerificationResult.Success;
        }

        public bool IsInPasswordHistory(User user, string newPassword)
        {
            var hasher = new PasswordHasher<User>();

            var recentHashes = _context.PasswordHistories //Check the most recent 5 password changes, ignore the rest after 5
                .Where(p => p.UserId == user.Id)
                .OrderByDescending(p => p.ChangedAt)
                .Take(5)
                .ToList();

            foreach (var old in recentHashes)
            {
                var result = hasher.VerifyHashedPassword(user, old.HashedPassword, newPassword);
                if (result == PasswordVerificationResult.Success)
                {
                    return true; // Reused password detected
                }
            }

            return false;
        }


        public void SavePasswordHistory(int userId, string newHashedPassword)
        {
            var history = new PasswordHistory
            {
                UserId = userId,
                HashedPassword = newHashedPassword,
                ChangedAt = DateTime.UtcNow
            };

            _context.PasswordHistories.Add(history);
            _context.SaveChanges();
        }
    }
}
