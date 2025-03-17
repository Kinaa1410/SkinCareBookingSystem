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
        public IFormFile ImageFile { get; set; } // For the image file upload
    }

    public class UpdateImageServiceDTO
    {
        [Required]
        public int ServiceId { get; set; }

        public IFormFile? ImageFile { get; set; } // Image file is optional for update
    }
}
