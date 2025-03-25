using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Controllers
{
    [Route("api/feedbacks")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [HttpGet("{serviceId}")]
        public async Task<IActionResult> GetFeedbackByServiceId(int serviceId)
        {
            var feedback = await _feedbackService.GetFeedbackByServiceIdAsync(serviceId);
            if (feedback == null) return NotFound("Feedback not found.");
            return Ok(feedback);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFeedbacks()
        {
            var feedbacks = await _feedbackService.GetAllFeedbacksAsync();
            return Ok(feedbacks);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFeedback([FromBody] CreateFeedbackDTO feedbackDTO)
        {
            if (feedbackDTO == null)
                return BadRequest("Invalid feedback data.");

            try
            {
                var feedback = await _feedbackService.CreateFeedbackAsync(feedbackDTO);
                return CreatedAtAction(nameof(GetFeedbackByServiceId), new { serviceId = feedback.ServiceId }, feedback);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{feedbackId}")]
        public async Task<IActionResult> UpdateFeedback(int feedbackId, [FromBody] UpdateFeedbackDTO feedbackDTO)
        {
            if (feedbackDTO == null)
                return BadRequest("Invalid feedback data.");

            var result = await _feedbackService.UpdateFeedbackAsync(feedbackId, feedbackDTO);
            if (!result) return NotFound("Feedback not found.");
            return Ok("Feedback is updated");
        }

        [HttpDelete("{feedbackId}")]
        public async Task<IActionResult> DeleteFeedback(int feedbackId)
        {
            var result = await _feedbackService.DeleteFeedbackAsync(feedbackId);
            if (!result) return NotFound("Feedback not found.");
            return Ok("Feedback is deleted");
        }
    }
}