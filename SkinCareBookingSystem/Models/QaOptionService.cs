using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkinCareBookingSystem.Models
{
    public class QaOptionService
    {
        [Key]
        public int QaOptionServiceId { get; set; }

        [ForeignKey("QaOption")]
        public int QaOptionId { get; set; }

        [ForeignKey("Service")]
        public int ServiceId { get; set; }

        public QaOption QaOption { get; set; } = null!;

        public Service Service { get; set; } = null!;
    }
}