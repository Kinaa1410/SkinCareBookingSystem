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

        public BookingService(BookingDbContext context)
        {
            _context = context;
        }

        // Check if a time slot is available
        private async Task<bool> IsTimeSlotAvailableAsync(int timeSlotId)
        {
            var timeSlot = await _context.TherapistTimeSlots
                .FirstOrDefaultAsync(ts => ts.TimeSlotId == timeSlotId && ts.Status == SlotStatus.Available);

            return timeSlot != null && timeSlot.Status == SlotStatus.Available;
        }

        // Get a booking by ID
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
                TherapistId = booking.TherapistTimeSlot.TherapistSchedule.TherapistUser.UserId,
                DateCreated = booking.DateCreated,
                TotalPrice = booking.TotalPrice,
                Status = booking.Status,
                IsPaid = booking.IsPaid,
                AppointmentDate = booking.AppointmentDate,
                UseWallet = booking.UseWallet
            };
        }

        // Get all bookings
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
                    TherapistId = b.TherapistTimeSlot.TherapistSchedule.TherapistUser.UserId,
                    DateCreated = b.DateCreated,
                    TotalPrice = b.TotalPrice,
                    Status = b.Status,
                    IsPaid = b.IsPaid,
                    AppointmentDate = b.AppointmentDate,
                    UseWallet = b.UseWallet
                }).ToListAsync();
        }

        // Create a new booking
        public async Task<BookingDTO> CreateBookingAsync(CreateBookingDTO bookingDTO)
        {
            var service = await _context.Services.FirstOrDefaultAsync(s => s.ServiceId == bookingDTO.ServiceId);
            if (service == null) throw new InvalidOperationException("Service not found.");

            var timeSlot = await _context.TherapistTimeSlots.FindAsync(bookingDTO.TimeSlotId);
            if (timeSlot == null || timeSlot.Status != SlotStatus.Available)
                throw new InvalidOperationException("Selected time slot is not available.");

            timeSlot.Status = SlotStatus.InProcess;

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

        // Update an existing booking
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

        // Delete a booking
        public async Task<bool> DeleteBookingAsync(int bookingId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
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

        // Book with a random available therapist
        public async Task<BookingDTO> BookWithRandomTherapistAsync(CreateBookingDTO bookingDTO)
        {
            var availableTimeSlot = await _context.TherapistTimeSlots
                .Where(ts => ts.Status == SlotStatus.Available)
                .OrderBy(r => Guid.NewGuid())  // Randomly order the available slots
                .FirstOrDefaultAsync();

            if (availableTimeSlot == null)
                throw new InvalidOperationException("No available time slots.");

            bookingDTO.TimeSlotId = availableTimeSlot.TimeSlotId;
            return await CreateBookingAsync(bookingDTO);
        }

        // Book with a specific therapist
        public async Task<BookingDTO> BookWithSpecificTherapistAsync(CreateBookingDTO bookingDTO)
        {
            var availableTimeSlot = await _context.TherapistTimeSlots
                .FirstOrDefaultAsync(ts => ts.TherapistSchedule.TherapistId == bookingDTO.TherapistId && ts.Status == SlotStatus.Available);

            if (availableTimeSlot == null)
                throw new InvalidOperationException("No available time slots for this therapist.");

            bookingDTO.TimeSlotId = availableTimeSlot.TimeSlotId;
            return await CreateBookingAsync(bookingDTO);
        }
    }
}
