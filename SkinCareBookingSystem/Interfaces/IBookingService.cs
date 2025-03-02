using SkinCareBookingSystem.DTOs;

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
        Task<BookingDTO> BookWithSpecificTherapistAsync(CreateBookingDTO bookingDTO);}
}
