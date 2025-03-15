using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Interfaces
{
    public interface IUserDetailsService
    {
        Task<IEnumerable<UserDetailsDTO>> GetAllUserDetailsAsync();
        Task<UserDetailsDTO> GetUserDetailsByIdAsync(int userId);
        Task<UserDetailsDTO> CreateUserDetailsAsync(CreateUserDetailsDTO userDetailsDTO, IFormFile avatarFile);
        Task<bool> UpdateUserDetailsAsync(int userId, UpdateUserDetailsDTO userDetailsDTO, IFormFile avatarFile);
        Task<bool> DeleteUserDetailsAsync(int userId);
    }
}
