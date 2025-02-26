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

        private async Task<bool> IsTherapistAvailableAsync(int therapistId, DateTime appointmentDate)
        {
            var appointmentTime = appointmentDate.TimeOfDay;

            var schedule = await _context.TherapistSchedules
                .FirstOrDefaultAsync(s => s.TherapistUser.Id == therapistId &&
                                          s.DayOfWeek == appointmentDate.DayOfWeek &&
                                          s.StartTime <= appointmentTime &&
                                          s.EndTime >= appointmentTime &&
                                          s.IsAvailable);

            return schedule != null && schedule.IsAvailable;
        }




        public async Task<BookingDTO> GetBookingByIdAsync(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.TherapistSchedule)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null) return null;

            return new BookingDTO
            {
                BookingId = booking.BookingId,
                UserId = booking.UserId,
                TherapistId = booking.TherapistSchedule.TherapistUser.Id,
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
                .Include(b => b.TherapistSchedule)
                .Select(b => new BookingDTO
                {
                    BookingId = b.BookingId,
                    UserId = b.UserId,
                    TherapistId = b.TherapistSchedule.TherapistUser.Id,
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
            bool isAvailable = await IsTherapistAvailableAsync((int)bookingDTO.TherapistId, bookingDTO.AppointmentDate);
            if (!isAvailable)
                throw new InvalidOperationException("Therapist is not available at the selected time.");

            var service = await _context.Services.FirstOrDefaultAsync(s => s.ServiceId == bookingDTO.ServiceId);
            if (service == null) throw new InvalidOperationException("Service not found.");

            var scheduleId = bookingDTO.ScheduleId;
            var schedule = await _context.TherapistSchedules.FindAsync(scheduleId);
            if (schedule == null || !schedule.IsAvailable)
                throw new InvalidOperationException("Schedule not available.");

            var booking = new Booking
            {
                UserId = bookingDTO.UserId,
                ScheduleId = schedule.ScheduleId,
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

            return new BookingDTO
            {
                BookingId = booking.BookingId,
                UserId = booking.UserId,
                TherapistId = booking.TherapistSchedule.TherapistUser.Id,
                DateCreated = booking.DateCreated,
                TotalPrice = booking.TotalPrice,
                Status = booking.Status,
                IsPaid = booking.IsPaid,
                AppointmentDate = booking.AppointmentDate,
                UseWallet = booking.UseWallet
            };
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

            var schedule = await _context.TherapistSchedules.FindAsync(booking.ScheduleId);
            if (schedule != null)
            {
                schedule.IsAvailable = true;
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return true;
        }

        // Scenario 1: Customer chooses random therapist for the chosen date and time
        public async Task<BookingDTO> BookWithRandomTherapistAsync(CreateBookingDTO bookingDTO)
        {
            var availableTherapists = await _context.Users
                .Where(u => u.RoleId == 2 && !u.CustomerBookings.Any(b => b.AppointmentDate == bookingDTO.AppointmentDate))
                .ToListAsync();

            if (!availableTherapists.Any())
                throw new InvalidOperationException("No therapists available for the selected time.");

            var randomTherapist = availableTherapists[new Random().Next(availableTherapists.Count)];

            bookingDTO.TherapistId = randomTherapist.Id;

            return await CreateBookingAsync(bookingDTO);
        }

        // Scenario 2: Customer chooses a specific therapist for the chosen date and time
        public async Task<BookingDTO> BookWithSpecificTherapistAsync(CreateBookingDTO bookingDTO)
        {
            var therapist = await _context.Users.FindAsync(bookingDTO.TherapistId);
            if (therapist == null || therapist.RoleId != 2)
                throw new InvalidOperationException("Therapist not found or invalid.");
            bool isAvailable = await IsTherapistAvailableAsync((int)bookingDTO.TherapistId, bookingDTO.AppointmentDate);
            if (!isAvailable)
                throw new InvalidOperationException("Therapist is not available at the selected time.");
            return await CreateBookingAsync(bookingDTO);
        }

        // Scenario 3: Customer chooses a specific therapist on the date and time they are free
        public async Task<BookingDTO> BookWithSpecificTherapistIfFreeAsync(CreateBookingDTO bookingDTO)
        {
            var therapist = await _context.Users.FindAsync(bookingDTO.TherapistId);
            if (therapist == null || therapist.RoleId != 2)
                throw new InvalidOperationException("Therapist not found or invalid.");

            bool isAvailable = await IsTherapistAvailableAsync((int)bookingDTO.TherapistId, bookingDTO.AppointmentDate);
            if (!isAvailable)
                throw new InvalidOperationException("Therapist is not available at the selected time.");

            return await CreateBookingAsync(bookingDTO);
        }

        // Scenario 4: Customer lets the staff choose everything for them
        public async Task<BookingDTO> BookWithStaffChoiceAsync(CreateBookingDTO bookingDTO)
        {
            var staff = await _context.Users.FindAsync(bookingDTO.UserId);
            if (staff == null || staff.RoleId != 3)
                throw new UnauthorizedAccessException("Only staff can book on behalf of a customer.");

            var availableTherapist = await _context.Users
                .Where(u => u.RoleId == 3 && !u.CustomerBookings.Any(b => b.AppointmentDate == bookingDTO.AppointmentDate))
                .FirstOrDefaultAsync();

            if (availableTherapist == null)
                throw new InvalidOperationException("No therapists available.");

            bookingDTO.TherapistId = availableTherapist.Id;

            return await CreateBookingAsync(bookingDTO);
        }
    }
}
