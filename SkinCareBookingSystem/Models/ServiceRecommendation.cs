using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkinCareBookingSystem.Models
{
    public class ServiceRecommendation
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Qa")]
        public int QaId { get; set; }

        [Required]
        public string AnswerOption { get; set; } = string.Empty;

        [ForeignKey("Service")]
        public int ServiceId { get; set; }

        public int Weight { get; set; }

        public Qa Qa { get; set; } = null!;
        public Service Service { get; set; } = null!;
    }
}
