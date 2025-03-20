using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.Data;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;
using SkinCareBookingSystem.Models;
using SkinCareBookingSystem.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

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

        private async Task<bool> IsTimeSlotAvailableAsync(int therapistTimeSlotId, DateTime date)
        {
            var timeSlot = await _context.TherapistTimeSlots
                .FirstOrDefaultAsync(ts => ts.Id == therapistTimeSlotId && ts.Status == SlotStatus.Available);
            if (timeSlot == null) return false;

            return !await _context.Bookings
                .AnyAsync(b => b.TimeSlotId == therapistTimeSlotId && b.AppointmentDate.Date == date.Date);
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
            var existingBooking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.AppointmentDate == bookingDTO.AppointmentDate && b.TimeSlotId == bookingDTO.TimeSlotId);

            if (existingBooking != null) throw new InvalidOperationException("Booking already exists for this date and time slot.");

            var service = await _context.Services.FirstOrDefaultAsync(s => s.ServiceId == bookingDTO.ServiceId);
            if (service == null) throw new InvalidOperationException("Service not found.");

            if (!await IsTimeSlotAvailableAsync(bookingDTO.TimeSlotId, bookingDTO.AppointmentDate))
                throw new InvalidOperationException("Time slot is not available.");

            await _therapistScheduleService.BookTimeSlotAsync(bookingDTO.TimeSlotId, bookingDTO.UserId);

            var booking = new Booking
            {
                UserId = bookingDTO.UserId,
                TimeSlotId = bookingDTO.TimeSlotId,
                DateCreated = DateTime.Now,
                TotalPrice = (float)service.Price,
                Status = true,
                IsPaid = false,
                UseWallet = bookingDTO.UseWallet,
                Note = bookingDTO.Note,
                AppointmentDate = bookingDTO.AppointmentDate.Date
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
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingId);
            if (booking == null) return false;

            var timeSlot = await _context.TherapistTimeSlots.FindAsync(booking.TimeSlotId);
            if (timeSlot != null)
            {
                timeSlot.Status = SlotStatus.Available;
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<BookingDTO> BookWithRandomTherapistAsync(CreateBookingDTO bookingDTO)
        {
            var availableTimeSlot = await _context.TherapistTimeSlots
                .Where(ts => ts.Status == SlotStatus.Available &&
                             !_context.Bookings.Any(b => b.TimeSlotId == ts.Id && b.AppointmentDate == bookingDTO.AppointmentDate))
                .OrderBy(r => Guid.NewGuid())
                .FirstOrDefaultAsync();

            if (availableTimeSlot == null)
                throw new InvalidOperationException("No available time slots.");

            await _therapistScheduleService.BookTimeSlotAsync(availableTimeSlot.Id, bookingDTO.UserId);
            bookingDTO.TimeSlotId = availableTimeSlot.Id;
            return await CreateBookingAsync(bookingDTO);
        }

        public async Task<BookingDTO> BookWithSpecificTherapistAsync(CreateBookingDTO bookingDTO)
        {
            var availableTimeSlot = await _context.TherapistTimeSlots
                .FirstOrDefaultAsync(ts => ts.TherapistSchedule.TherapistId == bookingDTO.TherapistId &&
                                          ts.Status == SlotStatus.Available &&
                                          !_context.Bookings.Any(b => b.TimeSlotId == ts.Id && b.AppointmentDate == bookingDTO.AppointmentDate));

            if (availableTimeSlot == null)
                throw new InvalidOperationException("No available time slots for this therapist.");

            await _therapistScheduleService.BookTimeSlotAsync(availableTimeSlot.Id, bookingDTO.UserId);
            bookingDTO.TimeSlotId = availableTimeSlot.Id;
            return await CreateBookingAsync(bookingDTO);
        }
    }
}