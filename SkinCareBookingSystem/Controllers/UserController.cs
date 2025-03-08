using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IValidator<CreateUserDTO> _createUserValidator;

        public UserController(IUserService userService, IValidator<CreateUserDTO> createUserValidator)
        {
            _userService = userService;
            _createUserValidator = createUserValidator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound(new { message = "User not found" });
            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<UserDTO>> CreateUser([FromBody] CreateUserDTO userDTO)
        {
            var validationResult = await _createUserValidator.ValidateAsync(userDTO);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            if (await _userService.UserExistsAsync(userDTO.UserName, userDTO.Email))
            {
                return BadRequest(new { message = "Username or Email already exists" });
            }

            var newUser = await _userService.RegisterUserAsync(userDTO);
            if (newUser == null)
            {
                return BadRequest(new { message = "User creation failed" });
            }

            return CreatedAtAction(nameof(GetUser), new { id = newUser.UserId }, newUser);
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var deleted = await _userService.DeleteUserAsync(id);

            if (!deleted)
                return NotFound(new { message = "User not found or could not be deleted" });

            return Ok(new { message = "User deleted successfully" });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create-staff")]
        public async Task<ActionResult<UserDTO>> CreateStaff([FromBody] CreateUserDTO userDTO)
        {
            var newStaff = await _userService.CreateStaffAsync(userDTO);
            if (newStaff == null)
                return BadRequest(new { message = "Email already exists" });

            return CreatedAtAction(nameof(GetUser), new { id = newStaff.UserId }, newStaff);
        }

        [Authorize(Roles = "Admin, Staff")]
        [HttpPost("create-therapist")]
        public async Task<ActionResult<UserDTO>> CreateTherapist([FromBody] CreateUserDTO userDTO)
        {
            var newTherapist = await _userService.CreateTherapistAsync(userDTO);
            if (newTherapist == null)
                return BadRequest(new { message = "Email already exists" });

            return CreatedAtAction(nameof(GetUser), new { id = newTherapist.UserId }, newTherapist);
        }

        [HttpGet("by-role/{roleName}")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsersByRole(string roleName)
        {
            var users = await _userService.GetUsersByRoleNameAsync(roleName);

            if (users == null || users.Count == 0)
            {
                return NotFound(new { message = $"No users found for the role '{roleName}'." });
            }

            return Ok(users);
        }

        [HttpPut("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdateUserDTO updateUserDTO)
        {
            string message = await _userService.UpdatePasswordAsync(updateUserDTO);

            if (message == "Username or Email is incorrect or not found.")
            {
                return BadRequest(message);
            }
            return Ok(message);
        }
    }
}
