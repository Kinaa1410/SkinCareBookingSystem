using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkinCareBookingSystem.Models
{
    public class Qa
    {
        [Key]
        public int QaId { get; set; }

        [ForeignKey("ServiceCategory")]
        public int ServiceCategoryId { get; set; }

        [Required]
        public string Question { get; set; } = string.Empty; // ✅ Fixes CS8618 warning

        [Required]
        public string Type { get; set; } = string.Empty; // ✅ Fixes CS8618 warning

        public bool Status { get; set; }

        // Navigation Property
        public ServiceCategory ServiceCategory { get; set; } = null!; // ✅ Fixes CS8618 warning
    }
}
