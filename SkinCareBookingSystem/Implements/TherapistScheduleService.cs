using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.Data;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;
using SkinCareBookingSystem.Models;
using SkinCareBookingSystem.Enums;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Implements
{
    public class TherapistScheduleService : ITherapistScheduleService
    {
        private readonly BookingDbContext _context;
        private readonly ITherapistTimeSlotService _therapistTimeSlotService;
        private readonly ITimeSlotService _timeSlotService;

        public TherapistScheduleService(
            BookingDbContext context,
            ITherapistTimeSlotService therapistTimeSlotService,
            ITimeSlotService timeSlotService)
        {
            _context = context;
            _therapistTimeSlotService = therapistTimeSlotService;
            _timeSlotService = timeSlotService;
        }

        private static DateTime ConvertToVietnamTime(DateTime dateTime)
        {
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime.ToUniversalTime(), vietnamTimeZone);
        }

        public async Task<IEnumerable<TherapistScheduleDTO>> GetAllSchedulesAsync()
        {
            return await _context.TherapistSchedules
                .Include(ts => ts.TherapistUser)
                .Include(ts => ts.TimeSlots)
                .ThenInclude(ts => ts.TimeSlot)
                .Select(ts => new TherapistScheduleDTO
                {
                    ScheduleId = ts.ScheduleId,
                    TherapistId = ts.TherapistId,
                    TherapistName = ts.TherapistUser.UserName,
                    DayOfWeek = ts.DayOfWeek,
                    StartWorkingTime = ts.StartWorkingTime,
                    EndWorkingTime = ts.EndWorkingTime,
                    TimeSlots = ts.TimeSlots.Select(slot => new TherapistTimeSlotDTO
                    {
                        Id = slot.Id,
                        ScheduleId = slot.ScheduleId,
                        TimeSlotId = slot.TimeSlotId,
                        TimeSlotDescription = $"{slot.TimeSlot.StartTime} - {slot.TimeSlot.EndTime}",
                        Status = slot.Status
                    }).ToList()
                }).ToListAsync();
        }

        public async Task<TherapistScheduleDTO> GetScheduleByIdAsync(int scheduleId)
        {
            var schedule = await _context.TherapistSchedules
                .Include(ts => ts.TherapistUser)
                .Include(ts => ts.TimeSlots)
                .ThenInclude(ts => ts.TimeSlot)
                .FirstOrDefaultAsync(ts => ts.ScheduleId == scheduleId);

            if (schedule == null) return null;

            return new TherapistScheduleDTO
            {
                ScheduleId = schedule.ScheduleId,
                TherapistId = schedule.TherapistId,
                TherapistName = schedule.TherapistUser.UserName,
                DayOfWeek = schedule.DayOfWeek,
                StartWorkingTime = schedule.StartWorkingTime,
                EndWorkingTime = schedule.EndWorkingTime,
                TimeSlots = schedule.TimeSlots.Select(slot => new TherapistTimeSlotDTO
                {
                    Id = slot.Id,
                    ScheduleId = slot.ScheduleId,
                    TimeSlotId = slot.TimeSlotId,
                    TimeSlotDescription = $"{slot.TimeSlot.StartTime} - {slot.TimeSlot.EndTime}",
                    Status = slot.Status
                }).ToList()
            };
        }

        public async Task<TherapistScheduleDTO> CreateScheduleAsync(CreateTherapistScheduleDTO scheduleDTO)
        {
            var therapist = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == scheduleDTO.TherapistId && u.RoleId == 2);

            if (therapist == null)
                throw new InvalidOperationException($"Therapist with ID {scheduleDTO.TherapistId} not found.");

            var schedule = new TherapistSchedule
            {
                TherapistId = scheduleDTO.TherapistId,
                DayOfWeek = scheduleDTO.DayOfWeek,
                StartWorkingTime = TimeSpan.Parse(scheduleDTO.StartTime),
                EndWorkingTime = TimeSpan.Parse(scheduleDTO.EndTime)
            };

            _context.TherapistSchedules.Add(schedule);
            await _context.SaveChangesAsync();

            await _therapistTimeSlotService.CreateTimeSlotForTherapistAsync(schedule.ScheduleId);

            return await GetScheduleByIdAsync(schedule.ScheduleId);
        }

        public async Task<bool> BookTimeSlotAsync(int therapistTimeSlotId, int userId, DateTime appointmentDate, int therapistId)
        {
            try
            {
                Console.WriteLine($"Checking TherapistTimeSlotId: {therapistTimeSlotId}, Date: {appointmentDate}, TherapistId: {therapistId}");

                var expectedDayOfWeek = appointmentDate.DayOfWeek;
                var timeSlot = await _context.TherapistTimeSlots
                    .Include(ts => ts.TherapistSchedule)
                    .Include(ts => ts.TimeSlot)
                    .FirstOrDefaultAsync(ts => ts.Id == therapistTimeSlotId &&
                                              ts.TherapistSchedule.TherapistId == therapistId &&
                                              ts.TherapistSchedule.DayOfWeek == expectedDayOfWeek);

                if (timeSlot == null)
                    throw new InvalidOperationException("Time slot does not exist for the specified therapist on this day.");

                if (timeSlot.TimeSlot.StartTime < timeSlot.TherapistSchedule.StartWorkingTime ||
                    timeSlot.TimeSlot.EndTime > timeSlot.TherapistSchedule.EndWorkingTime)
                    throw new InvalidOperationException($"Time slot {timeSlot.TimeSlot.StartTime} - {timeSlot.TimeSlot.EndTime} is outside the therapist's working hours: {timeSlot.TherapistSchedule.StartWorkingTime} - {timeSlot.TherapistSchedule.EndWorkingTime}.");

                var overlappingSlot = await _context.TherapistTimeSlots
                    .Include(ts => ts.TherapistSchedule)
                    .Include(ts => ts.TimeSlot)
                    .Where(ts => ts.TherapistSchedule.TherapistId == therapistId &&
                                 ts.Id != timeSlot.Id &&
                                 ts.TherapistSchedule.DayOfWeek == expectedDayOfWeek)
                    .Join(_context.Bookings,
                        ts => ts.Id,
                        b => b.TherapistTimeSlotId,
                        (ts, b) => new { TherapistTimeSlot = ts, Booking = b })
                    .Where(x => x.Booking.AppointmentDate.Date == appointmentDate.Date &&
                                (x.Booking.Status == BookingStatus.Pending || x.Booking.Status == BookingStatus.Booked) &&
                                ((x.TherapistTimeSlot.TimeSlot.StartTime >= timeSlot.TimeSlot.StartTime && x.TherapistTimeSlot.TimeSlot.StartTime < timeSlot.TimeSlot.EndTime) ||
                                 (x.TherapistTimeSlot.TimeSlot.EndTime > timeSlot.TimeSlot.StartTime && x.TherapistTimeSlot.TimeSlot.EndTime <= timeSlot.TimeSlot.EndTime)))
                    .FirstOrDefaultAsync();

                if (overlappingSlot != null)
                    throw new InvalidOperationException("Another time slot for this therapist overlaps with the requested time on this date.");

                var now = ConvertToVietnamTime(DateTime.UtcNow);
                if (appointmentDate.Date == now.Date)
                {
                    var currentTime = now.TimeOfDay;
                    if (currentTime > timeSlot.TimeSlot.StartTime)
                        throw new InvalidOperationException($"Cannot book a time slot in the past. Current time: {currentTime}, Slot start time: {timeSlot.TimeSlot.StartTime}.");
                }

                var isLocked = await _context.TherapistTimeSlotLocks
                    .AnyAsync(tsl => tsl.TherapistTimeSlotId == timeSlot.Id &&
                                     tsl.Date == appointmentDate.Date &&
                                     (tsl.Status == SlotStatus.InProcess || tsl.Status == SlotStatus.Booked));

                if (isLocked)
                    throw new InvalidOperationException("Time slot is already in process or booked for this date.");

                if (timeSlot.Status != SlotStatus.Available)
                    throw new InvalidOperationException($"Time slot is not available in the schedule. Current status: {timeSlot.Status}");

                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> UpdateScheduleAsync(int scheduleId, UpdateTherapistScheduleDTO scheduleDTO)
        {
            var schedule = await _context.TherapistSchedules
                .Include(ts => ts.TimeSlots)
                .ThenInclude(ts => ts.TimeSlot)
                .FirstOrDefaultAsync(ts => ts.ScheduleId == scheduleId);

            if (schedule == null) return false;

            bool isDayOfWeekChanged = schedule.DayOfWeek != scheduleDTO.DayOfWeek;
            bool isTimeRangeChanged = schedule.StartWorkingTime != TimeSpan.Parse(scheduleDTO.StartTime) ||
                                      schedule.EndWorkingTime != TimeSpan.Parse(scheduleDTO.EndTime);

            schedule.DayOfWeek = scheduleDTO.DayOfWeek;
            schedule.StartWorkingTime = TimeSpan.Parse(scheduleDTO.StartTime);
            schedule.EndWorkingTime = TimeSpan.Parse(scheduleDTO.EndTime);

            if (isDayOfWeekChanged || isTimeRangeChanged)
            {
                var oldTimeSlotsToRemove = schedule.TimeSlots.ToList();
                foreach (var timeSlot in oldTimeSlotsToRemove)
                {
                    await _therapistTimeSlotService.DeleteTimeSlotAsync(timeSlot.Id);
                }

                await _therapistTimeSlotService.CreateTimeSlotForTherapistAsync(scheduleId);
            }

            foreach (var updatedTimeSlot in scheduleDTO.TimeSlots)
            {
                await _therapistTimeSlotService.UpdateTimeSlotAsync(updatedTimeSlot.TimeSlotId, updatedTimeSlot.Status);
            }

            _context.Entry(schedule).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteScheduleAsync(int scheduleId)
        {
            var schedule = await _context.TherapistSchedules
                .Include(ts => ts.TimeSlots)
                .FirstOrDefaultAsync(ts => ts.ScheduleId == scheduleId);

            if (schedule == null) return false;

            foreach (var timeSlot in schedule.TimeSlots.ToList())
            {
                await _therapistTimeSlotService.DeleteTimeSlotAsync(timeSlot.Id);
            }

            _context.TherapistSchedules.Remove(schedule);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task SyncTimeSlotStatusesAsync(DateTime currentDate)
        {
            var timeSlots = await _context.TherapistTimeSlots.ToListAsync();
            foreach (var slot in timeSlots)
            {
                var hasFutureLocks = await _context.TherapistTimeSlotLocks
                    .AnyAsync(tsl => tsl.TherapistTimeSlotId == slot.Id &&
                                     tsl.Date >= currentDate.Date &&
                                     (tsl.Status == SlotStatus.InProcess || tsl.Status == SlotStatus.Booked));

                slot.Status = hasFutureLocks ? SlotStatus.Booked : SlotStatus.Available;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TherapistScheduleDTO>> GetTherapistsWorkingOnDayInTimeRangeAsync(DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime)
        {
            return await _context.TherapistSchedules
                .Where(ts => ts.DayOfWeek == dayOfWeek &&
                             ts.StartWorkingTime <= endTime &&
                             ts.EndWorkingTime >= startTime)
                .Include(ts => ts.TherapistUser)
                .Include(ts => ts.TimeSlots)
                .ThenInclude(ts => ts.TimeSlot)
                .Select(ts => new TherapistScheduleDTO
                {
                    ScheduleId = ts.ScheduleId,
                    TherapistId = ts.TherapistId,
                    TherapistName = ts.TherapistUser.UserName,
                    DayOfWeek = ts.DayOfWeek,
                    StartWorkingTime = ts.StartWorkingTime,
                    EndWorkingTime = ts.EndWorkingTime,
                    TimeSlots = ts.TimeSlots.Select(slot => new TherapistTimeSlotDTO
                    {
                        Id = slot.Id,
                        ScheduleId = slot.ScheduleId,
                        TimeSlotId = slot.TimeSlotId,
                        TimeSlotDescription = $"{slot.TimeSlot.StartTime} - {slot.TimeSlot.EndTime}",
                        Status = slot.Status
                    }).ToList()
                }).ToListAsync();
        }

        public async Task<IEnumerable<TherapistScheduleDTO>> GetTherapistsWorkingInTimeRangeAsync(TimeSpan startTime, TimeSpan endTime)
        {
            return await _context.TherapistSchedules
                .Where(ts => ts.StartWorkingTime <= endTime && ts.EndWorkingTime >= startTime)
                .Include(ts => ts.TherapistUser)
                .Include(ts => ts.TimeSlots)
                .ThenInclude(ts => ts.TimeSlot)
                .Select(ts => new TherapistScheduleDTO
                {
                    ScheduleId = ts.ScheduleId,
                    TherapistId = ts.TherapistId,
                    TherapistName = ts.TherapistUser.UserName,
                    DayOfWeek = ts.DayOfWeek,
                    StartWorkingTime = ts.StartWorkingTime,
                    EndWorkingTime = ts.EndWorkingTime,
                    TimeSlots = ts.TimeSlots.Select(slot => new TherapistTimeSlotDTO
                    {
                        Id = slot.Id,
                        ScheduleId = slot.ScheduleId,
                        TimeSlotId = slot.TimeSlotId,
                        TimeSlotDescription = $"{slot.TimeSlot.StartTime} - {slot.TimeSlot.EndTime}",
                        Status = slot.Status
                    }).ToList()
                }).ToListAsync();
        }

        public async Task<IEnumerable<TherapistScheduleDTO>> GetScheduleByTherapistIdAsync(int therapistId)
        {
            var schedules = await _context.TherapistSchedules
                .Where(ts => ts.TherapistId == therapistId)
                .Include(ts => ts.TherapistUser)
                .Include(ts => ts.TimeSlots)
                .ThenInclude(ts => ts.TimeSlot)
                .Select(ts => new TherapistScheduleDTO
                {
                    ScheduleId = ts.ScheduleId,
                    TherapistId = ts.TherapistId,
                    TherapistName = ts.TherapistUser.UserName,
                    DayOfWeek = ts.DayOfWeek,
                    StartWorkingTime = ts.StartWorkingTime,
                    EndWorkingTime = ts.EndWorkingTime,
                    TimeSlots = ts.TimeSlots.Select(slot => new TherapistTimeSlotDTO
                    {
                        Id = slot.Id,
                        ScheduleId = slot.ScheduleId,
                        TimeSlotId = slot.TimeSlotId,
                        TimeSlotDescription = $"{slot.TimeSlot.StartTime} - {slot.TimeSlot.EndTime}",
                        Status = slot.Status
                    }).ToList()
                }).ToListAsync();

            return schedules;
        }

        public async Task ResetCompletedBookingsAsync()
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var currentTime = ConvertToVietnamTime(DateTime.UtcNow);
                var bookedBookings = await _context.Bookings
                    .Include(b => b.TherapistTimeSlot)
                    .ThenInclude(ts => ts.TimeSlot)
                    .Where(b => b.Status == BookingStatus.Booked && b.IsPaid)
                    .ToListAsync();

                foreach (var booking in bookedBookings)
                {
                    if (booking.TherapistTimeSlot?.TimeSlot == null)
                        continue;

                    var endTimeOnDate = booking.AppointmentDate.Date
                        .Add(booking.TherapistTimeSlot.TimeSlot.EndTime);

                    var resetTime = endTimeOnDate.AddHours(1);

                    if (currentTime >= resetTime)
                    {
                        booking.Status = BookingStatus.Completed;

                        var timeSlotLock = await _context.TherapistTimeSlotLocks
                            .FirstOrDefaultAsync(tsl => tsl.TherapistTimeSlotId == booking.TherapistTimeSlotId &&
                                                        tsl.Date == booking.AppointmentDate.Date);

                        if (timeSlotLock != null)
                        {
                            _context.TherapistTimeSlotLocks.Remove(timeSlotLock);
                        }

                        _context.Entry(booking).Property(b => b.Status).IsModified = true;

                        Console.WriteLine($"Reset Booking {booking.BookingId}: Slot {booking.TherapistTimeSlotId} unlocked. EndTime: {endTimeOnDate}, ResetTime: {resetTime}");
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error resetting completed bookings: {ex.Message}");
                throw;
            }
        }

        public async Task<List<TherapistTimeSlotDTO>> GetAvailableSlotsAsync(int therapistId, DateTime date)
        {
            var expectedDayOfWeek = date.DayOfWeek;
            var availableSlots = await _context.TherapistTimeSlots
                .Include(ts => ts.TherapistSchedule)
                .Include(ts => ts.TimeSlot)
                .Where(ts => ts.TherapistSchedule.TherapistId == therapistId &&
                             ts.TherapistSchedule.DayOfWeek == expectedDayOfWeek &&
                             ts.Status == SlotStatus.Available)
                .GroupJoin(_context.TherapistTimeSlotLocks,
                    ts => ts.Id,
                    tsl => tsl.TherapistTimeSlotId,
                    (ts, locks) => new { TherapistTimeSlot = ts, Locks = locks })
                .SelectMany(x => x.Locks.DefaultIfEmpty(), (x, tsl) => new { x.TherapistTimeSlot, Lock = tsl })
                .Where(x => x.Lock == null || x.Lock.Date != date.Date ||
                            (x.Lock.Status != SlotStatus.InProcess && x.Lock.Status != SlotStatus.Booked))
                .Select(x => new TherapistTimeSlotDTO
                {
                    Id = x.TherapistTimeSlot.Id,
                    ScheduleId = x.TherapistTimeSlot.ScheduleId,
                    TimeSlotId = x.TherapistTimeSlot.TimeSlotId,
                    TimeSlotDescription = $"{x.TherapistTimeSlot.TimeSlot.StartTime} - {x.TherapistTimeSlot.TimeSlot.EndTime}",
                    Status = x.TherapistTimeSlot.Status
                })
                .Distinct()
                .ToListAsync();

            return availableSlots;
        }

        public async Task MarkSlotPermanentlyUnavailableAsync(int therapistTimeSlotId)
        {
            await _therapistTimeSlotService.UpdateTimeSlotAsync(therapistTimeSlotId, SlotStatus.Booked);
        }

        public async Task<TherapistTimeSlotDTO> GetTherapistTimeSlotByIdAsync(int therapistTimeSlotId)
        {
            return await _therapistTimeSlotService.GetTimeSlotByIdAsync(therapistTimeSlotId);
        }

        public async Task<TherapistTimeSlotDTO> CreateTherapistTimeSlotAsync(CreateTherapistTimeSlotDTO timeSlotDTO)
        {
            return await _therapistTimeSlotService.CreateTherapistTimeSlotAsync(timeSlotDTO);
        }

        public async Task<bool> UpdateTherapistTimeSlotAsync(int therapistTimeSlotId, UpdateTherapistTimeSlotDTO timeSlotDTO)
        {
            var timeSlot = await _context.TherapistTimeSlots
                .FirstOrDefaultAsync(ts => ts.Id == therapistTimeSlotId);

            if (timeSlot == null) return false;

            timeSlot.ScheduleId = timeSlotDTO.ScheduleId;
            timeSlot.TimeSlotId = timeSlotDTO.TimeSlotId;
            timeSlot.Status = timeSlotDTO.Status;

            _context.Entry(timeSlot).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteTherapistTimeSlotAsync(int therapistTimeSlotId)
        {
            return await _therapistTimeSlotService.DeleteTimeSlotAsync(therapistTimeSlotId);
        }

        private TimeSpan RoundToNextHour(TimeSpan time)
        {
            if (time.Minutes > 0)
            {
                return new TimeSpan(time.Hours + 1, 0, 0);
            }

            return time;
        }
    }
}