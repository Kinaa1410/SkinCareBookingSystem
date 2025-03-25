using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkinCareBookingSystem.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }

        [ForeignKey("Service")]
        public int ServiceId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; } = 0;

        [Required]
        public string Comment { get; set; } = string.Empty;

        public Service Service { get; set; } = null!;
    }
}