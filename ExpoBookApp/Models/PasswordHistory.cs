using System.ComponentModel.DataAnnotations;

namespace ExpoBookApp.Models
{
    public class PasswordHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        [Required]
        public string HashedPassword { get; set; }
        public DateTime ChangedAt { get; set; }
    }

}
