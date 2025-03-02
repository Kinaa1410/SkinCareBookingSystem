using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.Data;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;
using SkinCareBookingSystem.Models;

namespace SkinCareBookingSystem.Implements
{
    public class BookingService : IBookingService
    {
        private readonly BookingDbContext _context;

        public BookingService(BookingDbContext context)
        {
            _context = context;
        }

        private async Task<bool> IsTimeSlotAvailableAsync(int timeSlotId)
        {
            var timeSlot = await _context.TherapistTimeSlots
                .FirstOrDefaultAsync(ts => ts.TimeSlotId == timeSlotId && ts.IsAvailable);

            return timeSlot != null && timeSlot.IsAvailable;
        }

        public async Task<BookingDTO> GetBookingByIdAsync(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.TherapistTimeSlot) 
                .ThenInclude(ts => ts.TherapistSchedule)
                .ThenInclude(ts => ts.TherapistUser)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null) return null;

            return new BookingDTO
            {
                BookingId = booking.BookingId,
                UserId = booking.UserId,
                TherapistId = booking.TherapistTimeSlot.TherapistSchedule.TherapistUser.Id,
                DateCreated = booking.DateCreated,
                TotalPrice = booking.TotalPrice,
                Status = booking.Status,
                IsPaid = booking.IsPaid,
                AppointmentDate = booking.AppointmentDate,
                UseWallet = booking.UseWallet
            };
        }

        public async Task<IEnumerable<BookingDTO>> GetAllBookingsAsync()
        {
            return await _context.Bookings
                .Include(b => b.TherapistTimeSlot) 
                .ThenInclude(ts => ts.TherapistSchedule)
                .ThenInclude(ts => ts.TherapistUser)
                .Select(b => new BookingDTO
                {
                    BookingId = b.BookingId,
                    UserId = b.UserId,
                    TherapistId = b.TherapistTimeSlot.TherapistSchedule.TherapistUser.Id,
                    DateCreated = b.DateCreated,
                    TotalPrice = b.TotalPrice,
                    Status = b.Status,
                    IsPaid = b.IsPaid,
                    AppointmentDate = b.AppointmentDate,
                    UseWallet = b.UseWallet
                }).ToListAsync();
        }

        public async Task<BookingDTO> CreateBookingAsync(CreateBookingDTO bookingDTO)
        {
            var service = await _context.Services.FirstOrDefaultAsync(s => s.ServiceId == bookingDTO.ServiceId);
            if (service == null) throw new InvalidOperationException("Service not found.");

            var timeSlot = await _context.TherapistTimeSlots.FindAsync(bookingDTO.TimeSlotId);
            if (timeSlot == null || !timeSlot.IsAvailable)
                throw new InvalidOperationException("Selected time slot is not available.");

            timeSlot.IsAvailable = false;

            var booking = new Booking
            {
                UserId = bookingDTO.UserId,
                TimeSlotId = timeSlot.TimeSlotId,
                DateCreated = DateTime.Now,
                TotalPrice = (float)service.Price,
                Status = true,
                IsPaid = false,
                AppointmentDate = bookingDTO.AppointmentDate,
                UseWallet = bookingDTO.UseWallet,
                Note = bookingDTO.Note
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return await GetBookingByIdAsync(booking.BookingId);
        }

        public async Task<bool> UpdateBookingAsync(int bookingId, UpdateBookingDTO bookingDTO)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null) return false;

            booking.Status = bookingDTO.Status;
            booking.IsPaid = bookingDTO.IsPaid;

            _context.Entry(booking).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteBookingAsync(int bookingId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null) return false;

            var timeSlot = await _context.TherapistTimeSlots.FindAsync(booking.TimeSlotId);
            if (timeSlot != null)
            {
                timeSlot.IsAvailable = true;
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<BookingDTO> BookWithRandomTherapistAsync(CreateBookingDTO bookingDTO)
        {
            var availableTimeSlot = await _context.TherapistTimeSlots
                .Where(ts => ts.IsAvailable)
                .OrderBy(r => Guid.NewGuid())
                .FirstOrDefaultAsync();

            if (availableTimeSlot == null)
                throw new InvalidOperationException("No available time slots.");

            bookingDTO.TimeSlotId = availableTimeSlot.TimeSlotId;
            return await CreateBookingAsync(bookingDTO);
        }

        public async Task<BookingDTO> BookWithSpecificTherapistAsync(CreateBookingDTO bookingDTO)
        {
            var availableTimeSlot = await _context.TherapistTimeSlots
                .FirstOrDefaultAsync(ts => ts.TherapistSchedule.TherapistId == bookingDTO.TherapistId && ts.IsAvailable);

            if (availableTimeSlot == null)
                throw new InvalidOperationException("No available time slots for this therapist.");

            bookingDTO.TimeSlotId = availableTimeSlot.TimeSlotId;
            return await CreateBookingAsync(bookingDTO);
        }
    }
}
