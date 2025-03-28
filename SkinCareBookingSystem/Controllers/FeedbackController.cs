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
            var feedbacks = await _feedbackService.GetFeedbackByServiceIdAsync(serviceId);
            return feedbacks.Any() ? Ok(feedbacks) : NotFound("Feedback not found.");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFeedbacks()
        {
            return Ok(await _feedbackService.GetAllFeedbacksAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CreateFeedback([FromBody] CreateFeedbackDTO feedbackDTO)
        {
            if (feedbackDTO == null) return BadRequest("Invalid feedback data.");
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
            if (feedbackDTO == null) return BadRequest("Invalid feedback data.");
            return await _feedbackService.UpdateFeedbackAsync(feedbackId, feedbackDTO) ? Ok("Feedback is updated") : NotFound("Feedback not found.");
        }

        [HttpDelete("{feedbackId}")]
        public async Task<IActionResult> DeleteFeedback(int feedbackId)
        {
            return await _feedbackService.DeleteFeedbackAsync(feedbackId) ? Ok("Feedback is deleted") : NotFound("Feedback not found.");
        }
    }
}