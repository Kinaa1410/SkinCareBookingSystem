using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IValidator<LoginDTO> _loginValidator;

        public AuthController(IUserService userService, IValidator<LoginDTO> loginValidator)
        {
            _userService = userService;
            _loginValidator = loginValidator;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var validationResult = await _loginValidator.ValidateAsync(loginDTO);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var response = await _userService.LoginAsync(loginDTO);
            if (response == null)
                return Unauthorized(new { message = "Invalid username or password" });

            return Ok(new { message = "Login successful", token = response.Token, userId = response.UserId });
        }
     
    }
}
