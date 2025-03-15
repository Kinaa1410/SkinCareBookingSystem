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

        public async Task<UserDetailsDTO> CreateUserDetailsAsync(CreateUserDetailsDTO userDetailsDTO, IFormFile avatarFile)
        {
            string avatarPath = null;

            if (avatarFile != null && avatarFile.Length > 0)
            {
                var fileName = Path.GetFileName(avatarFile.FileName);
                var filePath = Path.Combine(_imageDirectory, fileName);

                // Create directory if it does not exist
                if (!Directory.Exists(_imageDirectory))
                {
                    Directory.CreateDirectory(_imageDirectory);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await avatarFile.CopyToAsync(stream);
                }

                avatarPath = filePath;  // Save the image path to the database
            }

            var userDetails = new UserDetails
            {
                UserId = userDetailsDTO.UserId,
                FirstName = userDetailsDTO.FirstName,
                LastName = userDetailsDTO.LastName,
                Address = userDetailsDTO.Address,
                Gender = userDetailsDTO.Gender,
                Avatar = avatarPath
            };

            _context.UserDetails.Add(userDetails);
            await _context.SaveChangesAsync();

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
            var userDetails = await _context.UserDetails.FindAsync(userId);
            if (userDetails == null) return false;

            if (avatarFile != null && avatarFile.Length > 0)
            {
                var fileName = Path.GetFileName(avatarFile.FileName);
                var filePath = Path.Combine(_imageDirectory, fileName);

                // Create directory if it does not exist
                if (!Directory.Exists(_imageDirectory))
                {
                    Directory.CreateDirectory(_imageDirectory);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await avatarFile.CopyToAsync(stream);
                }

                userDetails.Avatar = filePath;  // Update avatar path
            }

            userDetails.FirstName = userDetailsDTO.FirstName;
            userDetails.LastName = userDetailsDTO.LastName;
            userDetails.Address = userDetailsDTO.Address;
            userDetails.Gender = userDetailsDTO.Gender;

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
    }
}
