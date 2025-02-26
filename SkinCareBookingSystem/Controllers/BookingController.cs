using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;
using SkinCareBookingSystem.Models;

namespace SkinCareBookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // Scenario 1: Customer chooses random therapist for the chosen date and time
        [HttpPost("random-therapist")]
        public async Task<IActionResult> BookWithRandomTherapist([FromBody] CreateBookingDTO bookingDTO)
        {
            try
            {
                var booking = await _bookingService.BookWithRandomTherapistAsync(bookingDTO);
                return CreatedAtAction(nameof(GetBooking), new { bookingId = booking.BookingId }, booking);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Scenario 2: Customer chooses a specific therapist for the chosen date and time
        [HttpPost("specific-therapist")]
        public async Task<IActionResult> BookWithSpecificTherapist([FromBody] CreateBookingDTO bookingDTO)
        {
            try
            {
                var booking = await _bookingService.BookWithSpecificTherapistAsync(bookingDTO);
                return CreatedAtAction(nameof(GetBooking), new { bookingId = booking.BookingId }, booking);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Scenario 3: Customer chooses a specific therapist on the date and time they are free
        [HttpPost("specific-therapist-free")]
        public async Task<IActionResult> BookWithSpecificTherapistIfFree([FromBody] CreateBookingDTO bookingDTO)
        {
            try
            {
                var booking = await _bookingService.BookWithSpecificTherapistIfFreeAsync(bookingDTO);
                return CreatedAtAction(nameof(GetBooking), new { bookingId = booking.BookingId }, booking);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Scenario 4: Customer lets the staff choose everything for them
        [HttpPost("staff-choice")]
        public async Task<IActionResult> BookWithStaffChoice([FromBody] CreateBookingDTO bookingDTO)
        {
            try
            {
                var booking = await _bookingService.BookWithStaffChoiceAsync(bookingDTO);
                return CreatedAtAction(nameof(GetBooking), new { bookingId = booking.BookingId }, booking);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{bookingId}")]
        public async Task<IActionResult> GetBooking(int bookingId)
        {
            var booking = await _bookingService.GetBookingByIdAsync(bookingId);
            if (booking == null)
            {
                return NotFound("Booking not found.");
            }
            return Ok(booking);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            return Ok(bookings);
        }

        [HttpPut("{bookingId}")]
        public async Task<IActionResult> UpdateBooking(int bookingId, [FromBody] UpdateBookingDTO bookingDTO)
        {
            var result = await _bookingService.UpdateBookingAsync(bookingId, bookingDTO);
            if (!result)
            {
                return NotFound("Booking not found.");
            }

            return NoContent();
        }

        [HttpDelete("{bookingId}")]
        public async Task<IActionResult> DeleteBooking(int bookingId)
        {
            var result = await _bookingService.DeleteBookingAsync(bookingId);
            if (!result)
            {
                return NotFound("Booking not found.");
            }

            return NoContent(); 
        }
    }
}
