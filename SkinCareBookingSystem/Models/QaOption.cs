using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace SkinCareBookingSystem.Models
{
    public class QaOption
    {
        [Key]
        public int QaOptionId { get; set; }

        [Required]
        public string AnswerText { get; set; } = string.Empty;

        [ForeignKey("Qa")]
        public int QaId { get; set; }

        public Qa Qa { get; set; } = null!;

        public List<QaOptionService> ServiceRecommendations { get; set; } = new List<QaOptionService>();
    }
}