using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<UserDTO> GetUserByIdAsync(int id);
        Task<UserDTO> RegisterUserAsync(CreateUserDTO userDTO); 
        Task<UserDTO> CreateStaffAsync(CreateUserDTO userDTO);   
        Task<UserDTO> CreateTherapistAsync(CreateUserDTO userDTO); 
        Task<bool> UpdateUserAsync(int id, UpdateUserDTO userDTO);
        Task<bool> DeleteUserAsync(int id);
        Task<LoginResponseDTO?> LoginAsync(LoginDTO loginDTO);
        Task<List<UserDTO>> GetUsersByRoleIdAsync(int roleId);
        Task<bool> UserExistsAsync(string userName, string email);
    }
}
