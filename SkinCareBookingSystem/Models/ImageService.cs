using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkinCareBookingSystem.Models
{
    public class ImageService
    {
        [Key]
        public int ImageServiceId { get; set; }

        [ForeignKey("Service")]
        public int ServiceId { get; set; }

        [Required]
        public string ImageURL { get; set; } = string.Empty;

        public Service Service { get; set; } = null!;
    }
}
