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

        public async Task<UserDetailsDTO> CreateUserDetailsAsync(CreateUserDetailsDTO userDetails, IFormFile? avatarFile)
        {
            if (!Directory.Exists(_imageDirectory))
            {
                Directory.CreateDirectory(_imageDirectory);
            }

            if (avatarFile != null && avatarFile.Length > 0)
            {
                var avatarPath = Path.Combine(_imageDirectory, avatarFile.FileName);

                // Save the avatar file to disk
                using (var stream = new FileStream(avatarPath, FileMode.Create))
                {
                    await avatarFile.CopyToAsync(stream);
                }

                // Store the relative file path in the Avatar field
                userDetails.Avatar = Path.Combine("images", avatarFile.FileName);
            }

            var userDetail = new UserDetails
            {
                UserId = userDetails.UserId,
                FirstName = userDetails.FirstName,
                LastName = userDetails.LastName,
                Address = userDetails.Address,
                Gender = userDetails.Gender,
                Avatar = userDetails.Avatar // Store the file path here
            };

            _context.UserDetails.Add(userDetail);
            await _context.SaveChangesAsync();

            return new UserDetailsDTO
            {
                UserId = userDetail.UserId,
                FirstName = userDetail.FirstName,
                LastName = userDetail.LastName,
                Address = userDetail.Address,
                Gender = userDetail.Gender,
                Avatar = userDetail.Avatar // Return the stored file path
            };
        }

        public async Task<UserDetailsDTO> UpdateUserDetailsAsync(UpdateUserDetailsDTO updateUserDetailsDTO, IFormFile? avatarFile)
        {
            var userDetails = await _context.UserDetails.FirstOrDefaultAsync(ud => ud.UserId == updateUserDetailsDTO.UserId);
            if (userDetails == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            userDetails.FirstName = updateUserDetailsDTO.FirstName;
            userDetails.LastName = updateUserDetailsDTO.LastName;
            userDetails.Address = updateUserDetailsDTO.Address;
            userDetails.Gender = updateUserDetailsDTO.Gender;

            if (avatarFile != null && avatarFile.Length > 0)
            {
                var avatarPath = Path.Combine(_imageDirectory, avatarFile.FileName);

                // Save the avatar file to disk
                using (var stream = new FileStream(avatarPath, FileMode.Create))
                {
                    await avatarFile.CopyToAsync(stream);
                }

                // Store the relative file path in the Avatar field
                userDetails.Avatar = Path.Combine("images", avatarFile.FileName);
            }

            await _context.SaveChangesAsync();

            return new UserDetailsDTO
            {
                UserId = userDetails.UserId,
                FirstName = userDetails.FirstName,
                LastName = userDetails.LastName,
                Address = userDetails.Address,
                Gender = userDetails.Gender,
                Avatar = userDetails.Avatar // Return the stored file path
            };
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
