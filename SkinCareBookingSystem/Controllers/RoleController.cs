using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;

namespace SkinCareBookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IValidator<CreateRoleDTO> _createValidator;
        private readonly IValidator<UpdateRoleDTO> _updateValidator;

        public RoleController(IRoleService roleService,
            IValidator<CreateRoleDTO> createValidator,
            IValidator<UpdateRoleDTO> updateValidator)
        {
            _roleService = roleService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(roles);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetRole(int roleId)
        {
            var role = await _roleService.GetRoleByIdAsync(roleId);
            if (role == null)
            {
                return NotFound("Role not found");
            }
            return Ok(role);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDTO roleDTO)
        {
            var validationResult = await _createValidator.ValidateAsync(roleDTO);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var role = await _roleService.CreateRoleAsync(roleDTO);
            return CreatedAtAction(nameof(GetRole), new { roleId = role.RoleId }, role);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{roleId}")]
        public async Task<IActionResult> UpdateRole(int roleId, [FromBody] UpdateRoleDTO roleDTO)
        {
            var validationResult = await _updateValidator.ValidateAsync(roleDTO);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var result = await _roleService.UpdateRoleAsync(roleId, roleDTO);
            if (!result)
            {
                return NotFound("Role not found");
            }

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{roleId}")]
        public async Task<IActionResult> DeleteRole(int roleId)
        {
            var result = await _roleService.DeleteRoleAsync(roleId);
            if (!result)
            {
                return NotFound("Role not found");
            }

            return NoContent();
        }
    }
}
