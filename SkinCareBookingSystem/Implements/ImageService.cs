using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.Data;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;
using System.IO;

namespace SkinCareBookingSystem.Implements
{
    public class ImageService : IImageService
    {
        private readonly BookingDbContext _context;
        private readonly string _imageDirectory;

        public ImageService(BookingDbContext context)
        {
            _context = context;
            _imageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Service Images");  // Save images in Service Images folder
        }

        public async Task<ImageServiceDTO> CreateImageServiceAsync(CreateImageServiceDTO imageServiceDTO)
        {
            // Check if the directory exists, if not, create it
            if (!Directory.Exists(_imageDirectory))
            {
                Directory.CreateDirectory(_imageDirectory);
            }

            // Generate a unique file name
            var fileName = Guid.NewGuid() + Path.GetExtension(imageServiceDTO.ImageFile.FileName);  // Unique filename
            var filePath = Path.Combine(_imageDirectory, fileName);

            // Save the image to disk
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageServiceDTO.ImageFile.CopyToAsync(stream);
            }

            // Create ImageService record in DB
            var imageService = new Models.ImageService
            {
                ServiceId = imageServiceDTO.ServiceId,
                ImageURL = Path.Combine("Service Images", fileName)  // Store relative path in DB
            };

            // Add the new record to the database
            _context.ImageServices.Add(imageService);
            await _context.SaveChangesAsync();

            // Return the created ImageService as a DTO
            return new ImageServiceDTO
            {
                ImageServiceId = imageService.ImageServiceId,
                ServiceId = imageService.ServiceId,
                ImageURL = imageService.ImageURL
            };
        }

        public async Task<ImageServiceDTO> UpdateImageServiceAsync(int imageServiceId, UpdateImageServiceDTO imageServiceDTO)
        {
            // Retrieve the ImageService record by imageServiceId using FirstOrDefaultAsync with the 'img' variable name
            var imageService = await _context.ImageServices
                .FirstOrDefaultAsync(img => img.ImageServiceId == imageServiceId);  // Corrected 'is' to 'img'

            if (imageService == null)
            {
                throw new KeyNotFoundException("Image service not found.");
            }

            // If a new image is uploaded, replace the old image
            if (imageServiceDTO.ImageFile != null)
            {
                // Generate a new file name
                var fileName = Guid.NewGuid() + Path.GetExtension(imageServiceDTO.ImageFile.FileName);
                var filePath = Path.Combine(_imageDirectory, fileName);

                // Save the new image to disk
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageServiceDTO.ImageFile.CopyToAsync(stream);
                }

                // Update the ImageURL with the new file path
                imageService.ImageURL = Path.Combine("Service Images", fileName);
            }

            // Update the ServiceId (if provided)
            imageService.ServiceId = imageServiceDTO.ServiceId;

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return the updated ImageService as a DTO
            return new ImageServiceDTO
            {
                ImageServiceId = imageService.ImageServiceId,
                ServiceId = imageService.ServiceId,
                ImageURL = imageService.ImageURL
            };
        }

        public async Task<bool> DeleteImageServiceAsync(int imageServiceId)
        {
            // Retrieve the ImageService record by imageServiceId using FirstOrDefaultAsync with the 'img' variable name
            var imageService = await _context.ImageServices
                .FirstOrDefaultAsync(img => img.ImageServiceId == imageServiceId);  // Corrected 'is' to 'img'

            if (imageService == null)
            {
                return false; // Image service not found
            }

            // Delete the image file from disk
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imageService.ImageURL);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);  // Delete the file from the file system
            }

            // Remove the ImageService record from the database
            _context.ImageServices.Remove(imageService);
            await _context.SaveChangesAsync();

            return true;  // Successfully deleted
        }

        public async Task<ImageServiceDTO> GetImageServiceAsync(int imageServiceId)
        {
            // Retrieve the ImageService record by imageServiceId using FirstOrDefaultAsync with the 'img' variable name
            var imageService = await _context.ImageServices
                .FirstOrDefaultAsync(img => img.ImageServiceId == imageServiceId);  // Corrected 'is' to 'img'

            if (imageService == null)
            {
                throw new KeyNotFoundException("Image service not found.");
            }

            // Return the ImageService details as a DTO
            return new ImageServiceDTO
            {
                ImageServiceId = imageService.ImageServiceId,
                ServiceId = imageService.ServiceId,
                ImageURL = imageService.ImageURL
            };
        }
    }
}
