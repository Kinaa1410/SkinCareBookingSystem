using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;

namespace SkinCareBookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDetailsController : ControllerBase
    {
        private readonly IUserDetailsService _userDetailsService;
        private readonly IValidator<CreateUserDetailsDTO> _createValidator;
        //private readonly IValidator<UpdateUserDetailsDTO> _updateValidator;

        public UserDetailsController(IUserDetailsService userDetailsService,
            IValidator<CreateUserDetailsDTO> createValidator
            //IValidator<UpdateUserDetailsDTO> updateValidator
            )
        {
            _userDetailsService = userDetailsService;
            _createValidator = createValidator;
            //_updateValidator = updateValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUserDetails()
        {
            var userDetails = await _userDetailsService.GetAllUserDetailsAsync();
            return Ok(userDetails);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserDetails(int userId)
        {
            var userDetails = await _userDetailsService.GetUserDetailsByIdAsync(userId);
            if (userDetails == null)
            {
                return NotFound("User details not found");
            }
            return Ok(userDetails);
        }

        [HttpPost("CreateUserDetail")]
        public async Task<IActionResult> CreateUserDetails([FromForm] CreateUserDetailsDTO userDetailsDTO, IFormFile? avatarFile)
        {
            var validationResult = await _createValidator.ValidateAsync(userDetailsDTO);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var userDetails = await _userDetailsService.CreateUserDetailsAsync(userDetailsDTO, avatarFile);
            return CreatedAtAction(nameof(GetUserDetails), new { userId = userDetails.UserId }, userDetails);
        }

        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUserDetails([FromForm] UpdateUserDetailsDTO updateUserDetailsDTO, IFormFile? avatarFile)
        {
            // Validate incoming update request directly
            //var validationResult = await _updateValidator.ValidateAsync(updateUserDetailsDTO);
            //if (!validationResult.IsValid)
            //{
              //  return BadRequest(validationResult.Errors);
            //}

            try
            {
                // Update the user details
                var updatedUserDetails = await _userDetailsService.UpdateUserDetailsAsync(updateUserDetailsDTO, avatarFile);
                return Ok(new { Message = "User details updated successfully.", UserDetails = updatedUserDetails });
            }
            catch (KeyNotFoundException)
            {
                return NotFound("User not found.");
            }
        }




        [Authorize(Roles = "Admin")]
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUserDetails(int userId)
        {
            var result = await _userDetailsService.DeleteUserDetailsAsync(userId);
            if (!result)
            {
                return NotFound("User details not found");
            }

            return NoContent();
        }
    }
}
