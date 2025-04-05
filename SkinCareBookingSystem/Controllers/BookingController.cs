using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;
using FluentValidation;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SkinCareBookingSystem.Controllers
{
    [Route("api/bookings")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IValidator<CreateBookingDTO> _createValidator;
        private readonly IValidator<UpdateBookingDTO> _updateValidator;

        public BookingController(
            IBookingService bookingService,
            IValidator<CreateBookingDTO> createValidator,
            IValidator<UpdateBookingDTO> updateValidator)
        {
            _bookingService = bookingService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet("{bookingId}")]
        public async Task<IActionResult> GetBooking(int bookingId)
        {
            if (bookingId <= 0)
                return BadRequest("Invalid booking ID.");

            var booking = await _bookingService.GetBookingByIdAsync(bookingId);
            if (booking == null)
                return NotFound("Booking not found.");

            return Ok(booking);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            return Ok(bookings);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDTO bookingDTO)
        {
            var validationResult = await _createValidator.ValidateAsync(bookingDTO);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            try
            {
                var booking = await _bookingService.CreateBookingAsync(bookingDTO);
                return CreatedAtAction(nameof(GetBooking), new { bookingId = booking.BookingId }, booking);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPut("{bookingId}")]
        public async Task<IActionResult> UpdateBooking(int bookingId, [FromBody] UpdateBookingDTO bookingDTO)
        {
            if (bookingId <= 0)
                return BadRequest("Invalid booking ID.");

            var validationResult = await _updateValidator.ValidateAsync(bookingDTO);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var result = await _bookingService.UpdateBookingAsync(bookingId, bookingDTO);
            if (!result)
                return NotFound("Booking not found.");

            return NoContent();
        }

        [HttpDelete("{bookingId}")]
        public async Task<IActionResult> DeleteBooking(int bookingId)
        {
            if (bookingId <= 0)
                return BadRequest("Invalid booking ID.");

            var result = await _bookingService.DeleteBookingAsync(bookingId);
            if (!result)
                return NotFound("Booking not found.");

            return NoContent();
        }

        [HttpPost("random-therapist")]
        public async Task<IActionResult> BookWithRandomTherapist([FromBody] CreateBookingDTO bookingDTO)
        {
            var validationResult = await _createValidator.ValidateAsync(bookingDTO);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            try
            {
                var booking = await _bookingService.BookWithRandomTherapistAsync(bookingDTO);
                return CreatedAtAction(nameof(GetBooking), new { bookingId = booking.BookingId }, booking);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("specific-therapist")]
        public async Task<IActionResult> BookWithSpecificTherapist([FromBody] CreateBookingDTO bookingDTO)
        {
            var validationResult = await _createValidator.ValidateAsync(bookingDTO);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            try
            {
                var booking = await _bookingService.BookWithSpecificTherapistAsync(bookingDTO);
                return CreatedAtAction(nameof(GetBooking), new { bookingId = booking.BookingId }, booking);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("therapist/{therapistId}/bookings")]
        public async Task<ActionResult<IEnumerable<BookingDTO>>> GetBookingsByTherapistId(int therapistId)
        {
            if (therapistId <= 0)
                return BadRequest("Invalid therapist ID.");

            var bookings = await _bookingService.GetBookingsByTherapistIdAsync(therapistId);
            return Ok(bookings);
        }

        [HttpGet("user/{userId}/bookings")]
        public async Task<ActionResult<IEnumerable<BookingDTO>>> GetBookingsByUserId(int userId)
        {
            if (userId <= 0)
                return BadRequest("Invalid user ID.");

            var bookings = await _bookingService.GetBookingsByUserIdAsync(userId);
            return Ok(bookings);
        }

        [HttpPost("{bookingId}/confirm-completed")]
        public async Task<IActionResult> ConfirmBookingCompleted(int bookingId, [FromQuery] int therapistId)
        {
            if (bookingId <= 0 || therapistId <= 0)
                return BadRequest("Invalid booking ID or therapist ID.");

            try
            {
                var result = await _bookingService.ConfirmBookingCompletedAsync(bookingId, therapistId);
                if (!result)
                    return NotFound("Booking not found.");

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}