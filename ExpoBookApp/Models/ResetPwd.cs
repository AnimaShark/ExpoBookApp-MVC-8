using System.ComponentModel.DataAnnotations;
using ExpoBookApp.Services;

namespace ExpoBookApp.Models
{
    public class ResetPwd
    {
        [Required]
        public string PasswordResetToken { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [ComplexPassword]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
