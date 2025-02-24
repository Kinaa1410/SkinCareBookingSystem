using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkinCareBookingSystem.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }

        [ForeignKey("Booking")]
        public int BookingId { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        [Range(1, 5)]
        public int RatingService { get; set; } = 0;

        [Range(1, 5)]
        public int RatingTherapist { get; set; } = 0;

        [Required]
        public string CommentService { get; set; } = string.Empty;

        [Required]
        public string CommentTherapist { get; set; } = string.Empty;

        public bool Status { get; set; } = true;

        public Booking Booking { get; set; } = null!;
    }
}
