using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpoBookApp.Models
{
    public class Venues
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Venue Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Venue Size (sq ft)")]
        public int Size { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string StateCode { get; set; } // e.g., SG, KL

        [Required]
        public string State { get; set; } // e.g., Selangor, Kuala Lumpur

        [Display(Name = "Venue Status")]
        public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;

        [ForeignKey("CreatedByUserId")]
        [ValidateNever]
        public User CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public int CreatedByUserId { get; set; }
    }

    public enum ApprovalStatus
    {
        Pending,
        Approved,
        Rejected
    }

}