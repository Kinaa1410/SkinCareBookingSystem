using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SkinCareBookingSystem.Data;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;
using SkinCareBookingSystem.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SkinCareBookingSystem.Implements
{
    public class UserService : IUserService
    {
        private readonly BookingDbContext _context;
        private readonly IConfiguration _configuration;

        public UserService(BookingDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            return await _context.Users.Include(u => u.Role)
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    Role = u.Role.RoleName,
                    Status = u.Status
                }).ToListAsync();
        }

        public async Task<UserDTO> GetUserByIdAsync(int id)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return null;

            return new UserDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role.RoleName,
                Status = user.Status
            };
        }

        public async Task<bool> UserExistsAsync(string userName, string email)
        {
            return await _context.Users
                .AnyAsync(u => u.UserName == userName || u.Email == email);
        }

        public async Task<UserDTO> RegisterUserAsync(CreateUserDTO userDTO)
        {
            if (await _context.Users.AnyAsync(u => u.UserName == userDTO.UserName || u.Email == userDTO.Email))
            {
                return null;
            }

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Customer");

            var user = new User
            {
                UserName = userDTO.UserName,
                Email = userDTO.Email,
                Password = userDTO.Password,
                Role = role,
                Status = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role.RoleName,
                Status = user.Status
            };
        }



        public async Task<UserDTO> CreateStaffAsync(CreateUserDTO userDTO)
        {
            if (await _context.Users.AnyAsync(u => u.UserName == userDTO.UserName || u.Email == userDTO.Email))
            {
                return null;
            }

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Staff");

            var staff = new User
            {
                UserName = userDTO.UserName,
                Email = userDTO.Email,
                Password = userDTO.Password,
                Role = role,
                Status = true
            };

            _context.Users.Add(staff);
            await _context.SaveChangesAsync();

            return new UserDTO
            {
                Id = staff.Id,
                UserName = staff.UserName,
                Email = staff.Email,
                Role = staff.Role.RoleName,
                Status = staff.Status
            };
        }


        public async Task<UserDTO> CreateTherapistAsync(CreateUserDTO userDTO)
        {
            if (await _context.Users.AnyAsync(u => u.UserName == userDTO.UserName || u.Email == userDTO.Email))
            {
                return null;
            }

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Therapist");
            if (role == null)
            {
                return null; 
            }

            var therapist = new User
            {
                UserName = userDTO.UserName,
                Email = userDTO.Email,
                Password = userDTO.Password,
                Role = role, 
                Status = true
            };

            _context.Users.Add(therapist);
            await _context.SaveChangesAsync();

            return new UserDTO
            {
                Id = therapist.Id,
                UserName = therapist.UserName,
                Email = therapist.Email,
                Role = therapist.Role.RoleName,
                Status = therapist.Status
            };
        }




        public async Task<bool> UpdateUserAsync(int id, UpdateUserDTO userDTO)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            user.UserName = userDTO.UserName;
            user.Email = userDTO.Email;
            user.Password = userDTO.Password;
            user.Status = userDTO.Status;

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<LoginResponseDTO?> LoginAsync(LoginDTO loginDTO)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserName == loginDTO.UserName && u.Password == loginDTO.Password);

            if (user == null) return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, user.UserName), 
            new Claim(ClaimTypes.Role, user.Role.RoleName)
        }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return new LoginResponseDTO
            {
                Token = tokenString,
                UserName = user.UserName,
                Role = user.Role.RoleName
            };
        }

        public async Task<List<UserDTO>> GetUsersByRoleNameAsync(string roleName)
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.Role.RoleName == roleName)  
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    Role = u.Role.RoleName,
                    Status = u.Status
                }).ToListAsync();

            return users;
        }

    }
}
