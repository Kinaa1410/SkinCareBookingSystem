using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkinCareBookingSystem.Models
{
    public class QaAnswer
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Key]
        [Column(Order = 2)]
        [ForeignKey("Qa")]
        public int QaId { get; set; }

        [Required]
        public string Answer { get; set; } = string.Empty;

        // Navigation Properties
        public User User { get; set; } = null!;
        public Qa Qa { get; set; } = null!;
    }
}
