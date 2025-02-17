using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDTO>> GetAllRolesAsync();
        Task<RoleDTO> GetRoleByIdAsync(int roleId);
        Task<RoleDTO> CreateRoleAsync(CreateRoleDTO roleDTO);
        Task<bool> UpdateRoleAsync(int roleId, UpdateRoleDTO roleDTO);
        Task<bool> DeleteRoleAsync(int roleId);
    }
}
