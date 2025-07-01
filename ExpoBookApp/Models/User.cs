using System.ComponentModel.DataAnnotations;
using ExpoBookApp.Services;

namespace ExpoBookApp.Models
{
    public class User
    {
        /// User Attributes
        public int Id { get; set; }
        //Required(ErrorMessage = "Username is required")]
        //[StringLength(50, ErrorMessage = "Maximum 50 characters")]
        //public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Role { get; set; }

        [Required]
        [ComplexPassword]
        public string PasswordHash { get; set; }

        /// Password Reset Attribute
        public string? PasswordResetToken { get; set; }
        public DateTime? TokenExpiry { get; set; }

        ///Account Activation Attribute
        public bool IsEmailConfirmed { get; set; } = false;
        public string? EmailConfirmationToken { get; set; }
        //public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
