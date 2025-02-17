using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.Data;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;
using SkinCareBookingSystem.Models;

namespace SkinCareBookingSystem.Implements
{
    public class RoleService : IRoleService
    {
        private readonly BookingDbContext _context;

        public RoleService(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RoleDTO>> GetAllRolesAsync()
        {
            return await _context.Roles
                .Select(role => new RoleDTO
                {
                    RoleId = role.RoleId,
                    RoleName = role.RoleName,
                    Status = role.Status
                }).ToListAsync();
        }

        public async Task<RoleDTO> GetRoleByIdAsync(int roleId)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleId == roleId);
            if (role == null) return null;

            return new RoleDTO
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName,
                Status = role.Status
            };
        }

        public async Task<RoleDTO> CreateRoleAsync(CreateRoleDTO roleDTO)
        {
            var role = new Role
            {
                RoleName = roleDTO.RoleName,
                Status = roleDTO.Status
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            return new RoleDTO
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName,
                Status = role.Status
            };
        }

        public async Task<bool> UpdateRoleAsync(int roleId, UpdateRoleDTO roleDTO)
        {
            var role = await _context.Roles.FindAsync(roleId);
            if (role == null) return false;

            role.RoleName = roleDTO.RoleName;
            role.Status = roleDTO.Status;

            _context.Entry(role).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRoleAsync(int roleId)
        {
            var role = await _context.Roles.FindAsync(roleId);
            if (role == null) return false;

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
