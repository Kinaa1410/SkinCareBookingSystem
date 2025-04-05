using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace SkinCareBookingSystem.Models
{
    public class Qa
    {
        [Key]
        public int QaId { get; set; }

        [ForeignKey("ServiceCategory")]
        public int ServiceCategoryId { get; set; }

        [Required]
        public string Question { get; set; } = string.Empty;

        [Required]
        public string Type { get; set; } = string.Empty;

        public bool Status { get; set; }

        public List<QaOption> Options { get; set; } = new List<QaOption>();

        public ServiceCategory ServiceCategory { get; set; } = null!;
    }
}