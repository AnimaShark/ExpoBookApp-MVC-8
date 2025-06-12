using System.ComponentModel.DataAnnotations;
using ExpoBookApp.Services;

namespace ExpoBookApp.Models
{
    public class User
    {
        /// <summary>
        /// User Details
        /// </summary>
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Role { get; set; }

        [Required]
        [ComplexPassword]
        public string PasswordHash { get; set; }

        /// <summary>
        /// Password Reset Attribute
        /// </summary>
        public string? PasswordResetToken { get; set; }

        public DateTime? TokenExpiry { get; set; }

    }
}
