using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;

namespace SkinCareBookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QaController : ControllerBase
    {
        private readonly IQaService _qaService;
        private readonly IValidator<CreateQaDTO> _createValidator;
        private readonly IValidator<UpdateQaDTO> _updateValidator;

        public QaController(IQaService qaService,
            IValidator<CreateQaDTO> createValidator,
            IValidator<UpdateQaDTO> updateValidator)
        {
            _qaService = qaService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllQas()
        {
            var qas = await _qaService.GetAllQasAsync();
            return Ok(qas);
        }

        [HttpGet("{qaId}")]
        public async Task<IActionResult> GetQa(int qaId)
        {
            var qa = await _qaService.GetQaByIdAsync(qaId);
            if (qa == null)
            {
                return NotFound("QA not found");
            }
            return Ok(qa);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateQa([FromBody] CreateQaDTO qaDTO)
        {
            var validationResult = await _createValidator.ValidateAsync(qaDTO);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var qa = await _qaService.CreateQaAsync(qaDTO);
            return CreatedAtAction(nameof(GetQa), new { qaId = qa.QaId }, qa);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{qaId}")]
        public async Task<IActionResult> UpdateQa(int qaId, [FromBody] UpdateQaDTO qaDTO)
        {
            var validationResult = await _updateValidator.ValidateAsync(qaDTO);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var result = await _qaService.UpdateQaAsync(qaId, qaDTO);
            if (!result)
            {
                return NotFound("QA not found");
            }

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{qaId}")]
        public async Task<IActionResult> DeleteQa(int qaId)
        {
            var result = await _qaService.DeleteQaAsync(qaId);
            if (!result)
            {
                return NotFound("QA not found");
            }

            return NoContent();
        }
    }
}
