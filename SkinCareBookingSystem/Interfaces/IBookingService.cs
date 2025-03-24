using SkinCareBookingSystem.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SkinCareBookingSystem.Interfaces
{
    public interface IBookingService
    {
        // Get booking by booking ID
        Task<BookingDTO> GetBookingByIdAsync(int bookingId);

        // Get all bookings
        Task<IEnumerable<BookingDTO>> GetAllBookingsAsync();

        // Create a new booking
        Task<BookingDTO> CreateBookingAsync(CreateBookingDTO bookingDTO);

        // Update an existing booking
        Task<bool> UpdateBookingAsync(int bookingId, UpdateBookingDTO bookingDTO);

        // Delete a booking
        Task<bool> DeleteBookingAsync(int bookingId);

        // Book with a random available therapist
        Task<BookingDTO> BookWithRandomTherapistAsync(CreateBookingDTO bookingDTO);

        // Book with a specific therapist
        Task<BookingDTO> BookWithSpecificTherapistAsync(CreateBookingDTO bookingDTO);

        Task<IEnumerable<BookingDTO>> GetBookingsByTherapistIdAsync(int therapistId);
    }
}
