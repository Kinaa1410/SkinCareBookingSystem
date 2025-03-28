using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.Data;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;
using SkinCareBookingSystem.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using SkinCareBookingSystem.Enums;

namespace SkinCareBookingSystem.Implements
{
    public class BookingService : IBookingService
    {
        private readonly BookingDbContext _context;
        private readonly ITherapistScheduleService _therapistScheduleService;

        public BookingService(BookingDbContext context, ITherapistScheduleService therapistScheduleService)
        {
            _context = context;
            _therapistScheduleService = therapistScheduleService;
        }

        private async Task<DayOfWeek> GetTherapistDayOfWeekFromTimeSlotAsync(int timeSlotId)
        {
            var therapistTimeSlot = await _context.TherapistTimeSlots
                .Include(ts => ts.TherapistSchedule)
                .FirstOrDefaultAsync(ts => ts.TimeSlotId == timeSlotId);

            if (therapistTimeSlot == null || therapistTimeSlot.TherapistSchedule == null)
                throw new InvalidOperationException("Therapist time slot or schedule not found.");

            return therapistTimeSlot.TherapistSchedule.DayOfWeek;
        }

        // Updated method to allow same-day booking based on TimeSlot StartTime
        private async Task<DateTime> GetNextDateOfDayOfWeek(DateTime startDate, DayOfWeek targetDay, int timeSlotId)
        {
            var timeSlot = await _context.TherapistTimeSlots
                .Include(ts => ts.TimeSlot)
                .FirstOrDefaultAsync(ts => ts.TimeSlotId == timeSlotId);
            if (timeSlot == null || timeSlot.TimeSlot == null)
                throw new InvalidOperationException("Time slot details not found.");

            TimeSpan slotStartTime = timeSlot.TimeSlot.StartTime;
            int daysToAdd = ((int)targetDay - (int)startDate.DayOfWeek + 7) % 7;
            var candidateDate = startDate.Date.AddDays(daysToAdd);

            // If today is the target day, check if the slot's start time is still in the future
            if (candidateDate == startDate.Date && startDate.TimeOfDay >= slotStartTime)
                daysToAdd += 7; // Move to next week if slot time has passed

            return startDate.Date.AddDays(daysToAdd);
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
                TherapistId = booking.TherapistTimeSlot.TherapistSchedule.TherapistId,
                TimeSlotId = booking.TimeSlotId,
                DateCreated = booking.DateCreated,
                TotalPrice = booking.TotalPrice,
                Note = booking.Note,
                Status = booking.Status,
                IsPaid = booking.IsPaid,
                UseWallet = booking.UseWallet,
                AppointmentDate = booking.AppointmentDate
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
                    TherapistId = b.TherapistTimeSlot.TherapistSchedule.TherapistId,
                    TimeSlotId = b.TimeSlotId,
                    DateCreated = b.DateCreated,
                    TotalPrice = b.TotalPrice,
                    Note = b.Note,
                    Status = b.Status,
                    IsPaid = b.IsPaid,
                    UseWallet = b.UseWallet,
                    AppointmentDate = b.AppointmentDate
                }).ToListAsync();
        }

        public async Task<BookingDTO> CreateBookingAsync(CreateBookingDTO bookingDTO)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                Console.WriteLine($"Creating booking with TimeSlotId: {bookingDTO.TimeSlotId}, TherapistId: {bookingDTO.TherapistId}");

                var therapistTimeSlot = await _context.TherapistTimeSlots
                    .Include(ts => ts.TherapistSchedule)
                    .FirstOrDefaultAsync(ts => ts.TimeSlotId == bookingDTO.TimeSlotId && ts.TherapistSchedule.TherapistId == bookingDTO.TherapistId);

                if (therapistTimeSlot == null || therapistTimeSlot.TherapistSchedule == null)
                    throw new InvalidOperationException("Therapist time slot or schedule not found for the specified therapist.");

                var service = await _context.Services.FirstOrDefaultAsync(s => s.ServiceId == bookingDTO.ServiceId);
                if (service == null) throw new InvalidOperationException("Service not found.");

                DateTime appointmentDate = await GetNextDateOfDayOfWeek(DateTime.Now, therapistTimeSlot.TherapistSchedule.DayOfWeek, bookingDTO.TimeSlotId);

                if (appointmentDate.Date < DateTime.Now.Date)
                    throw new InvalidOperationException("Cannot book an appointment in the past.");

                await _therapistScheduleService.BookTimeSlotAsync(bookingDTO.TimeSlotId, bookingDTO.UserId, appointmentDate, (int)bookingDTO.TherapistId);

                var booking = new Booking
                {
                    UserId = bookingDTO.UserId,
                    TimeSlotId = bookingDTO.TimeSlotId,
                    DateCreated = DateTime.Now,
                    TotalPrice = (float)service.Price,
                    Status = BookingStatus.Pending,
                    IsPaid = false,
                    UseWallet = bookingDTO.UseWallet,
                    Note = bookingDTO.Note,
                    AppointmentDate = appointmentDate
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return await GetBookingByIdAsync(booking.BookingId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Booking creation failed: {ex.Message}");
                await transaction.RollbackAsync();
                throw;
            }
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
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingId);
            if (booking == null) return false;

            var timeSlot = await _context.TherapistTimeSlots.FindAsync(booking.TimeSlotId);
            if (timeSlot != null)
            {
                var hasOtherBookings = await _context.Bookings
                    .AnyAsync(b => b.TimeSlotId == booking.TimeSlotId && b.BookingId != bookingId &&
                                   b.Status != BookingStatus.Completed && b.Status != BookingStatus.Canceled && b.Status != BookingStatus.Failed);

                timeSlot.Status = hasOtherBookings ? SlotStatus.Booked : SlotStatus.Available;
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<BookingDTO> BookWithRandomTherapistAsync(CreateBookingDTO bookingDTO)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                DayOfWeek therapistDayOfWeek = await GetTherapistDayOfWeekFromTimeSlotAsync(bookingDTO.TimeSlotId);
                DateTime appointmentDate = await GetNextDateOfDayOfWeek(DateTime.Now, therapistDayOfWeek, bookingDTO.TimeSlotId);

                if (appointmentDate.Date < DateTime.Now.Date)
                    throw new InvalidOperationException("Cannot book an appointment in the past.");

                var availableTimeSlot = await _context.TherapistTimeSlots
                    .Include(ts => ts.TherapistSchedule)
                    .Where(ts => !_context.Bookings.Any(b => b.TimeSlotId == ts.Id && b.AppointmentDate.Date == appointmentDate.Date))
                    .OrderBy(r => Guid.NewGuid())
                    .FirstOrDefaultAsync();

                if (availableTimeSlot == null)
                    throw new InvalidOperationException("No available time slots.");

                if (availableTimeSlot.TherapistSchedule == null)
                    throw new InvalidOperationException("Therapist schedule not found for the selected time slot.");

                await _therapistScheduleService.BookTimeSlotAsync(availableTimeSlot.Id, bookingDTO.UserId, appointmentDate, availableTimeSlot.TherapistSchedule.TherapistId);
                bookingDTO.TimeSlotId = availableTimeSlot.Id;
                var result = await CreateBookingAsync(bookingDTO);

                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<BookingDTO> BookWithSpecificTherapistAsync(CreateBookingDTO bookingDTO)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                DayOfWeek therapistDayOfWeek = await GetTherapistDayOfWeekFromTimeSlotAsync(bookingDTO.TimeSlotId);
                DateTime appointmentDate = await GetNextDateOfDayOfWeek(DateTime.Now, therapistDayOfWeek, bookingDTO.TimeSlotId);

                if (appointmentDate.Date < DateTime.Now.Date)
                    throw new InvalidOperationException("Cannot book an appointment in the past.");

                var availableTimeSlot = await _context.TherapistTimeSlots
                    .Include(ts => ts.TherapistSchedule)
                    .FirstOrDefaultAsync(ts => ts.TherapistSchedule.TherapistId == bookingDTO.TherapistId &&
                                              !_context.Bookings.Any(b => b.TimeSlotId == ts.Id && b.AppointmentDate.Date == appointmentDate.Date));

                if (availableTimeSlot == null)
                    throw new InvalidOperationException("No available time slots for this therapist.");

                await _therapistScheduleService.BookTimeSlotAsync(availableTimeSlot.Id, bookingDTO.UserId, appointmentDate, (int)bookingDTO.TherapistId);
                bookingDTO.TimeSlotId = availableTimeSlot.Id;
                var result = await CreateBookingAsync(bookingDTO);

                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<BookingDTO>> GetBookingsByTherapistIdAsync(int therapistId)
        {
            return await _context.Bookings
                .Include(b => b.TherapistTimeSlot)
                .ThenInclude(ts => ts.TherapistSchedule)
                .Where(b => b.TherapistTimeSlot.TherapistSchedule.TherapistId == therapistId)
                .Select(b => new BookingDTO
                {
                    BookingId = b.BookingId,
                    UserId = b.UserId,
                    TherapistId = b.TherapistTimeSlot.TherapistSchedule.TherapistId,
                    TimeSlotId = b.TimeSlotId,
                    DateCreated = b.DateCreated,
                    TotalPrice = b.TotalPrice,
                    Note = b.Note,
                    Status = b.Status,
                    IsPaid = b.IsPaid,
                    UseWallet = b.UseWallet,
                    AppointmentDate = b.AppointmentDate
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<BookingDTO>> GetBookingsByUserIdAsync(int userId)
        {
            return await _context.Bookings
                .Include(b => b.TherapistTimeSlot)
                .ThenInclude(ts => ts.TherapistSchedule)
                .ThenInclude(ts => ts.TherapistUser)
                .Where(b => b.UserId == userId)
                .Select(b => new BookingDTO
                {
                    BookingId = b.BookingId,
                    UserId = b.UserId,
                    TherapistId = b.TherapistTimeSlot.TherapistSchedule.TherapistId,
                    TimeSlotId = b.TimeSlotId,
                    DateCreated = b.DateCreated,
                    TotalPrice = b.TotalPrice,
                    Note = b.Note,
                    Status = b.Status,
                    IsPaid = b.IsPaid,
                    UseWallet = b.UseWallet,
                    AppointmentDate = b.AppointmentDate
                })
                .ToListAsync();
        }
    }
}