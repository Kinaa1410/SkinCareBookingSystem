﻿using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<UserDTO> GetUserByIdAsync(int userId);
        Task<UserDTO> RegisterUserAsync(CreateUserDTO userDTO); 
        Task<UserDTO> CreateStaffAsync(CreateUserDTO userDTO);   
        Task<UserDTO> CreateTherapistAsync(CreateUserDTO userDTO); 
        Task<bool> UpdateUserAsync(int id, UpdateUserDTO userDTO);
        Task<bool> DeleteUserAsync(int id);
        Task<LoginResponseDTO?> LoginAsync(LoginDTO loginDTO);
        Task<List<UserDTO>> GetUsersByRoleNameAsync(string roleName);
        Task<bool> UserExistsAsync(string userName, string email);
        Task<string> UpdatePasswordAsync(UpdateUserDTO updateUserDTO);
    }
}
