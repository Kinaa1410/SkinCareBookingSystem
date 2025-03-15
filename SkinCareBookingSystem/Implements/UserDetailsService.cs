using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.Data;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;
using SkinCareBookingSystem.Models;
using System.IO;

namespace SkinCareBookingSystem.Implements
{
    public class UserDetailsService : IUserDetailsService
    {
        private readonly BookingDbContext _context;
        private readonly string _imageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

        public UserDetailsService(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserDetailsDTO>> GetAllUserDetailsAsync()
        {
            return await _context.UserDetails
                .Include(ud => ud.User)
                .Select(ud => new UserDetailsDTO
                {
                    UserId = ud.UserId,
                    FirstName = ud.FirstName,
                    LastName = ud.LastName,
                    Address = ud.Address,
                    Gender = ud.Gender,
                    Avatar = ud.Avatar
                }).ToListAsync();
        }

        public async Task<UserDetailsDTO> GetUserDetailsByIdAsync(int userId)
        {
            var userDetails = await _context.UserDetails
                .Include(ud => ud.User)
                .FirstOrDefaultAsync(ud => ud.UserId == userId);

            if (userDetails == null) return null;

            return new UserDetailsDTO
            {
                UserId = userDetails.UserId,
                FirstName = userDetails.FirstName,
                LastName = userDetails.LastName,
                Address = userDetails.Address,
                Gender = userDetails.Gender,
                Avatar = userDetails.Avatar
            };
        }

        public async Task<UserDetailsDTO> CreateUserDetailsAsync(CreateUserDetailsDTO userDetailsDTO, IFormFile? avatarFile)
        {
            string avatarPath = null;

            // Handle file upload if provided
            if (avatarFile?.Length > 0)
            {
                // Ensure the image directory exists
                Directory.CreateDirectory(_imageDirectory);

                // Generate a unique file name to prevent collisions
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(avatarFile.FileName);
                var filePath = Path.Combine(_imageDirectory, fileName);

                // Save the file to the server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await avatarFile.CopyToAsync(stream);
                }

                // Store the relative URL for the avatar image
                avatarPath = $"/images/{fileName}";
            }

            // Create the user details object
            var userDetails = new UserDetails
            {
                UserId = userDetailsDTO.UserId,
                FirstName = userDetailsDTO.FirstName,
                LastName = userDetailsDTO.LastName,
                Address = userDetailsDTO.Address,
                Gender = userDetailsDTO.Gender,
                Avatar = avatarPath // Null if no avatar file was uploaded
            };

            // Save to the database
            _context.UserDetails.Add(userDetails);
            await _context.SaveChangesAsync();

            // Return the DTO with the user details
            return new UserDetailsDTO
            {
                UserId = userDetails.UserId,
                FirstName = userDetails.FirstName,
                LastName = userDetails.LastName,
                Address = userDetails.Address,
                Gender = userDetails.Gender,
                Avatar = userDetails.Avatar
            };
        }



        public async Task<bool> UpdateUserDetailsAsync(int userId, UpdateUserDetailsDTO userDetailsDTO, IFormFile avatarFile)
        {
            // Find the user by their ID
            var userDetails = await _context.UserDetails.FindAsync(userId);
            if (userDetails == null) return false; // Return false if the user doesn't exist

            // Handle file upload if provided
            if (avatarFile?.Length > 0)
            {
                // Generate a unique file name for the avatar
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(avatarFile.FileName);
                var filePath = Path.Combine(_imageDirectory, fileName);

                // Ensure the image directory exists
                Directory.CreateDirectory(_imageDirectory);

                // Save the file to the server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await avatarFile.CopyToAsync(stream);
                }

                // Update the avatar field with the relative file path
                userDetails.Avatar = $"/images/{fileName}";
            }

            // Update other user details
            userDetails.FirstName = userDetailsDTO.FirstName;
            userDetails.LastName = userDetailsDTO.LastName;
            userDetails.Address = userDetailsDTO.Address;
            userDetails.Gender = userDetailsDTO.Gender;

            // Mark the entity as modified and save changes
            _context.Entry(userDetails).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<bool> DeleteUserDetailsAsync(int userId)
        {
            var userDetails = await _context.UserDetails.FindAsync(userId);
            if (userDetails == null) return false;

            _context.UserDetails.Remove(userDetails);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<byte[]> DownloadImageAsync(string url)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
                else
                {
                    throw new Exception("Failed to download the image.");
                }
            }
        }
    }
}
