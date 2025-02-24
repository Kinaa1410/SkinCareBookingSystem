using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;

namespace SkinCareBookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QaAnswerController : ControllerBase
    {
        private readonly IQaAnswerService _qaAnswerService;
        private readonly IValidator<CreateQaAnswerDTO> _createValidator;
        private readonly IValidator<UpdateQaAnswerDTO> _updateValidator;

        public QaAnswerController(IQaAnswerService qaAnswerService,
            IValidator<CreateQaAnswerDTO> createValidator,
            IValidator<UpdateQaAnswerDTO> updateValidator)
        {
            _qaAnswerService = qaAnswerService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAllQaAnswers()
        {
            var qaAnswers = await _qaAnswerService.GetAllQaAnswersAsync();
            return Ok(qaAnswers);
        }

        
        [HttpGet("{userId}/{qaId}")]
        public async Task<IActionResult> GetQaAnswer(int userId, int qaId)
        {
            var qaAnswer = await _qaAnswerService.GetQaAnswerByIdsAsync(userId, qaId);
            if (qaAnswer == null)
            {
                return NotFound("QA Answer not found");
            }
            return Ok(qaAnswer);
        }

        
        [HttpPost]
        public async Task<IActionResult> CreateQaAnswer([FromBody] CreateQaAnswerDTO qaAnswerDTO)
        {
            var validationResult = await _createValidator.ValidateAsync(qaAnswerDTO);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var qaAnswer = await _qaAnswerService.CreateQaAnswerAsync(qaAnswerDTO);
            return CreatedAtAction(nameof(GetQaAnswer), new { userId = qaAnswer.UserId, qaId = qaAnswer.QaId }, qaAnswer);
        }

        
        [HttpPut("{userId}/{qaId}")]
        public async Task<IActionResult> UpdateQaAnswer(int userId, int qaId, [FromBody] UpdateQaAnswerDTO qaAnswerDTO)
        {
            var validationResult = await _updateValidator.ValidateAsync(qaAnswerDTO);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var result = await _qaAnswerService.UpdateQaAnswerAsync(userId, qaId, qaAnswerDTO);
            if (!result)
            {
                return NotFound("QA Answer not found");
            }

            return NoContent();
        }

        
        [HttpDelete("{userId}/{qaId}")]
        public async Task<IActionResult> DeleteQaAnswer(int userId, int qaId)
        {
            var result = await _qaAnswerService.DeleteQaAnswerAsync(userId, qaId);
            if (!result)
            {
                return NotFound("QA Answer not found");
            }

            return NoContent();
        }
    }
}
