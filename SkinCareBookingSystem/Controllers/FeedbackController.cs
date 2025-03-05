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

        [HttpGet("{bookingId}")]
        public async Task<IActionResult> GetFeedbackByBookingId(int bookingId)
        {
            var feedback = await _feedbackService.GetFeedbackByBookingIdAsync(bookingId);
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
                return CreatedAtAction(nameof(GetFeedbackByBookingId), new { bookingId = feedback.BookingId }, feedback);
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


        [HttpGet("ratings/{therapistId}")]
        public async Task<IActionResult> GetTherapistRatings(int therapistId)
        {
            var ratings = await _feedbackService.GetTherapistRatingsAsync(therapistId);
            if (ratings == null || !ratings.Any()) return NotFound("No ratings found for this therapist.");
            return Ok(ratings);
        }

        [HttpGet("average-rating/{therapistId}")]
        public async Task<IActionResult> GetAverageTherapistRating(int therapistId)
        {
            var averageRating = await _feedbackService.GetAverageTherapistRatingAsync(therapistId);
            return Ok(new { AverageRating = averageRating });
        }
    }
}
