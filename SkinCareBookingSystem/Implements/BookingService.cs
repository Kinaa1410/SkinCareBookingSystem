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

        private static DateTime ConvertToVietnamTime(DateTime dateTime)
        {
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime.ToUniversalTime(), vietnamTimeZone);
        }

        public async Task<BookingDTO> GetBookingByIdAsync(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Therapist)
                .Include(b => b.Service)
                .Include(b => b.TherapistTimeSlot)
                .ThenInclude(ts => ts.TimeSlot)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null) return null;

            return new BookingDTO
            {
                BookingId = booking.BookingId,
                UserId = booking.UserId,
                TherapistId = booking.TherapistId,
                ServiceId = booking.ServiceId,
                TimeSlotId = booking.TimeSlotId,
                TherapistTimeSlotId = booking.TherapistTimeSlotId,
                DateCreated = ConvertToVietnamTime(booking.DateCreated),
                TotalPrice = booking.TotalPrice,
                Note = booking.Note,
                Status = booking.Status,
                IsPaid = booking.IsPaid,
                AppointmentDate = ConvertToVietnamTime(booking.AppointmentDate)
            };
        }

        public async Task<IEnumerable<BookingDTO>> GetAllBookingsAsync()
        {
            return await _context.Bookings
                .Include(b => b.Therapist)
                .Include(b => b.Service)
                .Include(b => b.TherapistTimeSlot)
                .ThenInclude(ts => ts.TimeSlot)
                .Select(b => new BookingDTO
                {
                    BookingId = b.BookingId,
                    UserId = b.UserId,
                    TherapistId = b.TherapistId,
                    ServiceId = b.ServiceId,
                    TimeSlotId = b.TimeSlotId,
                    TherapistTimeSlotId = b.TherapistTimeSlotId,
                    DateCreated = ConvertToVietnamTime(b.DateCreated),
                    TotalPrice = b.TotalPrice,
                    Note = b.Note,
                    Status = b.Status,
                    IsPaid = b.IsPaid,
                    AppointmentDate = ConvertToVietnamTime(b.AppointmentDate)
                }).ToListAsync();
        }

        public async Task<BookingDTO> CreateBookingAsync(CreateBookingDTO bookingDTO)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var therapist = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == bookingDTO.TherapistId);
                if (therapist == null)
                    throw new InvalidOperationException("Therapist not found.");

                var service = await _context.Services
                    .Include(s => s.ServiceCategory)
                    .FirstOrDefaultAsync(s => s.ServiceId == bookingDTO.ServiceId);
                if (service == null)
                    throw new InvalidOperationException("Service not found.");

                var therapistSpecialty = await _context.TherapistSpecialties
                    .AnyAsync(ts => ts.TherapistId == bookingDTO.TherapistId &&
                                  ts.ServiceCategoryId == service.ServiceCategoryId);
                if (!therapistSpecialty)
                    throw new InvalidOperationException("Therapist does not offer this service category.");

                DateTime appointmentDate = bookingDTO.AppointmentDate.Date;

                var expectedDayOfWeek = appointmentDate.DayOfWeek;
                var therapistTimeSlot = await _context.TherapistTimeSlots
                    .Include(ts => ts.TherapistSchedule)
                    .FirstOrDefaultAsync(ts => ts.TimeSlotId == bookingDTO.TimeSlotId &&
                                               ts.TherapistSchedule.TherapistId == bookingDTO.TherapistId &&
                                               ts.TherapistSchedule.DayOfWeek == expectedDayOfWeek);

                if (therapistTimeSlot == null)
                    throw new InvalidOperationException("Therapist time slot not found for this date.");

                var bookTimeSlotResult = await _therapistScheduleService.BookTimeSlotAsync(
                    therapistTimeSlot.Id,
                    bookingDTO.UserId,
                    appointmentDate,
                    bookingDTO.TherapistId);

                if (!bookTimeSlotResult)
                    throw new Exception("Unable to book timeslot");

                var booking = new Booking
                {
                    UserId = bookingDTO.UserId,
                    TherapistId = bookingDTO.TherapistId,
                    ServiceId = bookingDTO.ServiceId,
                    TimeSlotId = bookingDTO.TimeSlotId,
                    TherapistTimeSlotId = therapistTimeSlot.Id,
                    DateCreated = DateTime.Now,
                    TotalPrice = (float)service.Price,
                    Status = BookingStatus.Pending,
                    IsPaid = false,
                    Note = bookingDTO.Note,
                    AppointmentDate = appointmentDate
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await GetBookingByIdAsync(booking.BookingId);
            }
            catch (Exception)
            {
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

            var timeSlotLock = await _context.TherapistTimeSlotLocks
                .FirstOrDefaultAsync(tsl => tsl.TherapistTimeSlotId == booking.TherapistTimeSlotId &&
                                            tsl.Date == booking.AppointmentDate.Date);

            if (timeSlotLock != null)
            {
                _context.TherapistTimeSlotLocks.Remove(timeSlotLock);
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
                var service = await _context.Services
                    .Include(s => s.ServiceCategory)
                    .FirstOrDefaultAsync(s => s.ServiceId == bookingDTO.ServiceId);
                if (service == null)
                    throw new InvalidOperationException("Service not found.");

                DateTime appointmentDate = bookingDTO.AppointmentDate.Date;

                var availableTimeSlot = await _context.TherapistTimeSlots
                    .Include(ts => ts.TherapistSchedule)
                    .Include(ts => ts.TimeSlot)
                    .Where(ts => ts.TherapistSchedule.DayOfWeek == appointmentDate.DayOfWeek &&
                                 ts.Status == SlotStatus.Available &&
                                 ts.TimeSlotId == bookingDTO.TimeSlotId &&
                                 _context.TherapistSpecialties.Any(tsp => tsp.TherapistId == ts.TherapistSchedule.TherapistId &&
                                                                        tsp.ServiceCategoryId == service.ServiceCategoryId))
                    .GroupJoin(_context.TherapistTimeSlotLocks,
                        ts => ts.Id,
                        tsl => tsl.TherapistTimeSlotId,
                        (ts, locks) => new { TherapistTimeSlot = ts, Locks = locks })
                    .SelectMany(x => x.Locks.DefaultIfEmpty(), (x, tsl) => new { x.TherapistTimeSlot, Lock = tsl })
                    .Where(x => x.Lock == null || x.Lock.Date != appointmentDate.Date ||
                                (x.Lock.Status != SlotStatus.InProcess && x.Lock.Status != SlotStatus.Booked))
                    .OrderBy(r => Guid.NewGuid())
                    .Select(x => x.TherapistTimeSlot)
                    .FirstOrDefaultAsync();

                if (availableTimeSlot?.TherapistSchedule == null)
                    throw new InvalidOperationException("No available therapists for this service on the selected date.");

                bookingDTO.TherapistId = availableTimeSlot.TherapistSchedule.TherapistId;

                await _therapistScheduleService.BookTimeSlotAsync(
                    availableTimeSlot.Id,
                    bookingDTO.UserId,
                    appointmentDate,
                    bookingDTO.TherapistId);

                var result = await CreateBookingAsync(bookingDTO);

                await transaction.CommitAsync();
                return result;
            }
            catch (Exception)
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
                var therapist = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == bookingDTO.TherapistId);
                if (therapist == null)
                    throw new InvalidOperationException("Therapist not found.");

                var service = await _context.Services
                    .Include(s => s.ServiceCategory)
                    .FirstOrDefaultAsync(s => s.ServiceId == bookingDTO.ServiceId);
                if (service == null)
                    throw new InvalidOperationException("Service not found.");

                var therapistSpecialty = await _context.TherapistSpecialties
                    .AnyAsync(ts => ts.TherapistId == bookingDTO.TherapistId &&
                                  ts.ServiceCategoryId == service.ServiceCategoryId);
                if (!therapistSpecialty)
                    throw new InvalidOperationException("Therapist does not offer this service category.");

                DateTime appointmentDate = bookingDTO.AppointmentDate.Date;

                var availableTimeSlot = await _context.TherapistTimeSlots
                    .Include(ts => ts.TherapistSchedule)
                    .Include(ts => ts.TimeSlot)
                    .FirstOrDefaultAsync(ts => ts.TherapistSchedule.TherapistId == bookingDTO.TherapistId &&
                                               ts.TherapistSchedule.DayOfWeek == appointmentDate.DayOfWeek &&
                                               ts.TimeSlotId == bookingDTO.TimeSlotId &&
                                               ts.Status == SlotStatus.Available);

                if (availableTimeSlot == null)
                    throw new InvalidOperationException("No available time slots for this therapist on the selected date.");

                await _therapistScheduleService.BookTimeSlotAsync(
                    availableTimeSlot.Id,
                    bookingDTO.UserId,
                    appointmentDate,
                    bookingDTO.TherapistId);

                var result = await CreateBookingAsync(bookingDTO);

                await transaction.CommitAsync();
                return result;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<BookingDTO>> GetBookingsByTherapistIdAsync(int therapistId)
        {
            return await _context.Bookings
                .Include(b => b.Therapist)
                .Include(b => b.Service)
                .Include(b => b.TherapistTimeSlot)
                .ThenInclude(ts => ts.TimeSlot)
                .Where(b => b.TherapistId == therapistId)
                .Select(b => new BookingDTO
                {
                    BookingId = b.BookingId,
                    UserId = b.UserId,
                    TherapistId = b.TherapistId,
                    ServiceId = b.ServiceId,
                    TimeSlotId = b.TimeSlotId,
                    TherapistTimeSlotId = b.TherapistTimeSlotId,
                    DateCreated = ConvertToVietnamTime(b.DateCreated),
                    TotalPrice = b.TotalPrice,
                    Note = b.Note,
                    Status = b.Status,
                    IsPaid = b.IsPaid,
                    AppointmentDate = ConvertToVietnamTime(b.AppointmentDate)
                }).ToListAsync();
        }

        public async Task<IEnumerable<BookingDTO>> GetBookingsByUserIdAsync(int userId)
        {
            return await _context.Bookings
                .Include(b => b.Therapist)
                .Include(b => b.Service)
                .Include(b => b.TherapistTimeSlot)
                .ThenInclude(ts => ts.TimeSlot)
                .Where(b => b.UserId == userId)
                .Select(b => new BookingDTO
                {
                    BookingId = b.BookingId,
                    UserId = b.UserId,
                    TherapistId = b.TherapistId,
                    ServiceId = b.ServiceId,
                    TimeSlotId = b.TimeSlotId,
                    TherapistTimeSlotId = b.TherapistTimeSlotId,
                    DateCreated = ConvertToVietnamTime(b.DateCreated),
                    TotalPrice = b.TotalPrice,
                    Note = b.Note,
                    Status = b.Status,
                    IsPaid = b.IsPaid,
                    AppointmentDate = ConvertToVietnamTime(b.AppointmentDate)
                }).ToListAsync();
        }

        public async Task<bool> ConfirmBookingCompletedAsync(int bookingId, int therapistId)
        {
            var booking = await _context.Bookings
                .Include(b => b.TherapistTimeSlot)
                .ThenInclude(ts => ts.TimeSlot)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);
            if (booking == null)
                return false;

            if (booking.TherapistId != therapistId)
                throw new InvalidOperationException("Only the assigned therapist can confirm this booking.");

            if (booking.Status != BookingStatus.Booked)
                throw new InvalidOperationException("Booking can only be confirmed as completed if it is in Booked status.");

            var currentTime = ConvertToVietnamTime(DateTime.UtcNow);
            var bookingEndTime = booking.AppointmentDate.Date + booking.TherapistTimeSlot.TimeSlot.EndTime;
            if (currentTime < bookingEndTime)
                throw new InvalidOperationException($"Cannot confirm booking as completed before its end time: {bookingEndTime}");

            booking.Status = BookingStatus.Completed;

            var timeSlotLock = await _context.TherapistTimeSlotLocks
                .FirstOrDefaultAsync(tsl => tsl.TherapistTimeSlotId == booking.TherapistTimeSlotId &&
                                            tsl.Date == booking.AppointmentDate.Date);

            if (timeSlotLock != null)
            {
                _context.TherapistTimeSlotLocks.Remove(timeSlotLock);
            }

            _context.Entry(booking).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}