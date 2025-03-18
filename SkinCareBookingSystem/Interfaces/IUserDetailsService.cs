using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Interfaces
{
    public interface IUserDetailsService
    {
        Task<IEnumerable<UserDetailsDTO>> GetAllUserDetailsAsync();
        Task<UserDetailsDTO> GetUserDetailsByIdAsync(int userId);
        Task<UserDetailsDTO> CreateUserDetailsAsync(CreateUserDetailsDTO userDetails);
        Task<UserDetailsDTO> UpdateUserDetailsAsync(UpdateUserDetailsDTO updateUserDetailsDTO);
        Task<bool> DeleteUserDetailsAsync(int userId);
    }
}
