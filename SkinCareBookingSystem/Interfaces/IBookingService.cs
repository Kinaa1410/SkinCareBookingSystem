using SkinCareBookingSystem.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Interfaces
{
    public interface IBookingService
    {
        Task<BookingDTO> GetBookingByIdAsync(int bookingId);
        Task<IEnumerable<BookingDTO>> GetAllBookingsAsync();
        Task<BookingDTO> CreateBookingAsync(CreateBookingDTO bookingDTO);
        Task<bool> UpdateBookingAsync(int bookingId, UpdateBookingDTO bookingDTO);
        Task<bool> DeleteBookingAsync(int bookingId);
        Task<BookingDTO> BookWithRandomTherapistAsync(CreateBookingDTO bookingDTO);
        Task<BookingDTO> BookWithSpecificTherapistAsync(CreateBookingDTO bookingDTO);
        Task<IEnumerable<BookingDTO>> GetBookingsByTherapistIdAsync(int therapistId);
        Task<IEnumerable<BookingDTO>> GetBookingsByUserIdAsync(int userId);
        Task<bool> ConfirmBookingCompletedAsync(int bookingId, int therapistId);
    }
}