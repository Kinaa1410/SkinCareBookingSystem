using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Controllers
{
    [Route("api/bookings")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet("{bookingId}")]
        public async Task<IActionResult> GetBooking(int bookingId)
        {
            var booking = await _bookingService.GetBookingByIdAsync(bookingId);
            if (booking == null) return NotFound("Booking not found.");
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
            if (bookingDTO == null)
                return BadRequest("Invalid booking data.");

            try
            {
                var booking = await _bookingService.CreateBookingAsync(bookingDTO);
                return CreatedAtAction(nameof(GetBooking), new { bookingId = booking.BookingId }, booking);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut("{bookingId}")]
        public async Task<IActionResult> UpdateBooking(int bookingId, [FromBody] UpdateBookingDTO bookingDTO)
        {
            if (bookingDTO == null)
                return BadRequest("Invalid booking data.");

            var result = await _bookingService.UpdateBookingAsync(bookingId, bookingDTO);
            if (!result) return NotFound("Booking not found.");
            return NoContent();
        }

        [HttpDelete("{bookingId}")]
        public async Task<IActionResult> DeleteBooking(int bookingId)
        {
            var result = await _bookingService.DeleteBookingAsync(bookingId);
            if (!result) return NotFound("Booking not found.");
            return NoContent();
        }


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

        [HttpGet("therapist/{therapistId}/bookings")]
        public async Task<ActionResult<IEnumerable<BookingDTO>>> GetBookingsByTherapistId(int therapistId)
        {
            var bookings = await _bookingService.GetBookingsByTherapistIdAsync(therapistId);
            return Ok(bookings);
        }

        [HttpGet("user/{userId}/bookings")]
        public async Task<ActionResult<IEnumerable<BookingDTO>>> GetBookingsByUserId(int userId)
        {
            var bookings = await _bookingService.GetBookingsByUserIdAsync(userId);
            return Ok(bookings);
        }
    }
}
