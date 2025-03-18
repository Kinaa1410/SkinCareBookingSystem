using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.Data;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;

namespace SkinCareBookingSystem.Implements
{
    public class ImageService : IImageService
    {
        private readonly BookingDbContext _context;

        public ImageService(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<ImageServiceDTO> CreateImageServiceAsync(CreateImageServiceDTO imageServiceDTO)
        {
            var imageService = new Models.ImageService
            {
                ServiceId = imageServiceDTO.ServiceId,
                ImageURL = imageServiceDTO.ImageURL // Store Cloudinary URL directly
            };

            _context.ImageServices.Add(imageService);
            await _context.SaveChangesAsync();

            return new ImageServiceDTO
            {
                ImageServiceId = imageService.ImageServiceId,
                ServiceId = imageService.ServiceId,
                ImageURL = imageService.ImageURL
            };
        }

        public async Task<ImageServiceDTO> UpdateImageServiceAsync(int imageServiceId, UpdateImageServiceDTO imageServiceDTO)
        {
            var imageService = await _context.ImageServices
                .FirstOrDefaultAsync(img => img.ImageServiceId == imageServiceId);

            if (imageService == null)
            {
                throw new KeyNotFoundException("Image service not found.");
            }

            imageService.ServiceId = imageServiceDTO.ServiceId;

            if (!string.IsNullOrEmpty(imageServiceDTO.ImageURL))
            {
                imageService.ImageURL = imageServiceDTO.ImageURL; // Update with new Cloudinary URL if provided
            }

            await _context.SaveChangesAsync();

            return new ImageServiceDTO
            {
                ImageServiceId = imageService.ImageServiceId,
                ServiceId = imageService.ServiceId,
                ImageURL = imageService.ImageURL
            };
        }

        public async Task<bool> DeleteImageServiceAsync(int imageServiceId)
        {
            var imageService = await _context.ImageServices
                .FirstOrDefaultAsync(img => img.ImageServiceId == imageServiceId);

            if (imageService == null)
            {
                return false;
            }

            _context.ImageServices.Remove(imageService);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<ImageServiceDTO> GetImageServiceAsync(int imageServiceId)
        {
            var imageService = await _context.ImageServices
                .FirstOrDefaultAsync(img => img.ImageServiceId == imageServiceId);

            if (imageService == null)
            {
                throw new KeyNotFoundException("Image service not found.");
            }

            return new ImageServiceDTO
            {
                ImageServiceId = imageService.ImageServiceId,
                ServiceId = imageService.ServiceId,
                ImageURL = imageService.ImageURL
            };
        }
    }
}