using System.ComponentModel.DataAnnotations;

namespace SkinCareBookingSystem.DTOs
{
    public class ImageServiceDTO
    {
        public int ImageServiceId { get; set; }
        public int ServiceId { get; set; }
        public string ImageURL { get; set; } = string.Empty;
    }

    public class CreateImageServiceDTO
    {
        [Required]
        public int ServiceId { get; set; }

        [Required]
        public string ImageURL { get; set; } 
    }

    public class UpdateImageServiceDTO
    {
        [Required]
        public int ServiceId { get; set; }

        public string? ImageURL { get; set; }
    }
}