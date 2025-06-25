using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpoBookApp.Models
{
    public class Venue
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string LocationName { get; set; }

        [Required]
        public string LocationAddress { get; set; }

        [Range(0, int.MaxValue)]
        public int LocationSize { get; set; }

        [Range(0, int.MaxValue)]
        public int Capacity { get; set; }

        [Range(0.0, double.MaxValue)]
        public decimal PricePerDay { get; set; }

        public string? ImagePath { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        //Created By
        [ForeignKey("CreatedByUserId")]
        public User CreatedBy { get; set; }
        public int CreatedByUserId { get; set; }

        // Status of listing
        public VenueStatus Status { get; set; } = VenueStatus.Pending;
    }

    public enum VenueStatus
    {
        Pending,
        Approved,
        Rejected
    }
}