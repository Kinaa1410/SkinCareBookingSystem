using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.Data;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;
using SkinCareBookingSystem.Models;

namespace SkinCareBookingSystem.Implements
{
    public class UserDetailsService : IUserDetailsService
    {
        private readonly BookingDbContext _context;

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

            var isServiceImage = await _context.ImageServices.AnyAsync(img => img.ImageURL == userDetails.Avatar);
            if (isServiceImage)
            {
                userDetails.Avatar = null;
            }

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

        public async Task<UserDetailsDTO> CreateUserDetailsAsync(CreateUserDetailsDTO userDetails)
        {
            var userDetail = new UserDetails
            {
                UserId = userDetails.UserId,
                FirstName = userDetails.FirstName,
                LastName = userDetails.LastName,
                Address = userDetails.Address,
                Gender = userDetails.Gender,
                Avatar = userDetails.Avatar
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
                Avatar = userDetail.Avatar
            };
        }

        public async Task<UserDetailsDTO> UpdateUserDetailsAsync(UpdateUserDetailsDTO updateUserDetailsDTO)
        {
            var userDetails = await _context.UserDetails.FirstOrDefaultAsync(ud => ud.UserId == updateUserDetailsDTO.UserId);

            if (userDetails == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            if (!string.IsNullOrEmpty(updateUserDetailsDTO.FirstName))
            {
                userDetails.FirstName = updateUserDetailsDTO.FirstName;
            }

            if (!string.IsNullOrEmpty(updateUserDetailsDTO.LastName))
            {
                userDetails.LastName = updateUserDetailsDTO.LastName;
            }

            if (!string.IsNullOrEmpty(updateUserDetailsDTO.Address))
            {
                userDetails.Address = updateUserDetailsDTO.Address;
            }

            if (!string.IsNullOrEmpty(updateUserDetailsDTO.Gender))
            {
                userDetails.Gender = updateUserDetailsDTO.Gender;
            }

            if (!string.IsNullOrEmpty(updateUserDetailsDTO.Avatar))
            {
                userDetails.Avatar = updateUserDetailsDTO.Avatar;
            }

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