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

        public async Task<UserDetailsDTO> CreateUserDetailsAsync(CreateUserDetailsDTO userDetailsDTO)
        {
            var userDetails = new UserDetails
            {
                UserId = userDetailsDTO.UserId,
                FirstName = userDetailsDTO.FirstName,
                LastName = userDetailsDTO.LastName,
                Address = userDetailsDTO.Address,
                Gender = userDetailsDTO.Gender,
                Avatar = userDetailsDTO.Avatar
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

        public async Task<bool> UpdateUserDetailsAsync(int userId, UpdateUserDetailsDTO userDetailsDTO)
        {
            var userDetails = await _context.UserDetails.FindAsync(userId);
            if (userDetails == null) return false;

            userDetails.FirstName = userDetailsDTO.FirstName;
            userDetails.LastName = userDetailsDTO.LastName;
            userDetails.Address = userDetailsDTO.Address;
            userDetails.Gender = userDetailsDTO.Gender;
            userDetails.Avatar = userDetailsDTO.Avatar;

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
