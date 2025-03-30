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

        public TherapistScheduleService(BookingDbContext context)
        {
            _context = context;
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
                        TimeSlotId = slot.TimeSlotId,
                        TimeSlotDescription = slot.TimeSlot.Description,
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
                    TimeSlotId = slot.TimeSlotId,
                    TimeSlotDescription = slot.TimeSlot.Description,
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

            var startTime = TimeSpan.Parse(scheduleDTO.StartTime);
            var endTime = TimeSpan.Parse(scheduleDTO.EndTime);

            startTime = RoundToNextHour(startTime);
            endTime = RoundToNextHour(endTime);

            var timeSlots = new List<TherapistTimeSlot>();

            while (startTime < endTime)
            {
                var timeSlot = await _context.TimeSlots
                    .FirstOrDefaultAsync(ts => ts.StartTime == startTime && ts.EndTime == startTime.Add(new TimeSpan(1, 0, 0)));

                if (timeSlot != null)
                {
                    timeSlots.Add(new TherapistTimeSlot
                    {
                        TimeSlotId = timeSlot.TimeSlotId,
                        Status = SlotStatus.Available
                    });
                }
                else
                {
                    throw new InvalidOperationException($"Time slot for {startTime} to {startTime.Add(new TimeSpan(1, 0, 0))} does not exist.");
                }

                startTime = startTime.Add(new TimeSpan(1, 0, 0));
            }

            var schedule = new TherapistSchedule
            {
                TherapistId = scheduleDTO.TherapistId,
                DayOfWeek = scheduleDTO.DayOfWeek,
                StartWorkingTime = TimeSpan.Parse(scheduleDTO.StartTime),
                EndWorkingTime = TimeSpan.Parse(scheduleDTO.EndTime),
                TimeSlots = timeSlots
            };

            _context.TherapistSchedules.Add(schedule);
            await _context.SaveChangesAsync();

            return await GetScheduleByIdAsync(schedule.ScheduleId);
        }

        public async Task<bool> BookTimeSlotAsync(int timeSlotId, int userId, DateTime appointmentDate, int therapistId)
        {
            try
            {
                Console.WriteLine($"Checking TimeSlotId: {timeSlotId}, Date: {appointmentDate}, TherapistId: {therapistId}");

                var expectedDayOfWeek = appointmentDate.DayOfWeek;
                var timeSlot = await _context.TherapistTimeSlots
                    .Include(ts => ts.TherapistSchedule)
                    .Include(ts => ts.TimeSlot)
                    .FirstOrDefaultAsync(ts => ts.TimeSlotId == timeSlotId &&
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
                        b => b.TimeSlotId,
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

                var anyActiveBooking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.TimeSlotId == timeSlot.Id &&
                                                (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Booked));
                if (anyActiveBooking != null)
                    throw new InvalidOperationException("Time slot is marked Available but has an active booking.");

                var existingBooking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.AppointmentDate.Date == appointmentDate.Date &&
                                                b.TimeSlotId == timeSlot.Id &&
                                                (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Booked));
                if (existingBooking != null)
                    throw new InvalidOperationException("Time slot is already booked for this date.");

                if (timeSlot.Status != SlotStatus.Available)
                    throw new InvalidOperationException($"Time slot is not available. Current status: {timeSlot.Status}");

                timeSlot.Status = SlotStatus.InProcess;
                await _context.SaveChangesAsync();

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

            schedule.DayOfWeek = scheduleDTO.DayOfWeek;
            schedule.StartWorkingTime = TimeSpan.Parse(scheduleDTO.StartTime);
            schedule.EndWorkingTime = TimeSpan.Parse(scheduleDTO.EndTime);

            var newTimeSlots = new List<TherapistTimeSlot>();

            if (isDayOfWeekChanged)
            {
                var oldTimeSlotsToRemove = schedule.TimeSlots.ToList();
                _context.TherapistTimeSlots.RemoveRange(oldTimeSlotsToRemove);
            }

            var startTime = schedule.StartWorkingTime;
            var endTime = schedule.EndWorkingTime;

            while (startTime < endTime)
            {
                var timeSlot = await _context.TimeSlots
                    .FirstOrDefaultAsync(ts => ts.StartTime == startTime && ts.EndTime == startTime.Add(new TimeSpan(1, 0, 0)));

                if (timeSlot != null)
                {
                    var existingSlot = schedule.TimeSlots.FirstOrDefault(ts => ts.TimeSlotId == timeSlot.TimeSlotId);
                    if (existingSlot != null)
                    {
                        newTimeSlots.Add(new TherapistTimeSlot
                        {
                            TimeSlotId = timeSlot.TimeSlotId,
                            Status = existingSlot.Status
                        });
                    }
                    else
                    {
                        newTimeSlots.Add(new TherapistTimeSlot
                        {
                            TimeSlotId = timeSlot.TimeSlotId,
                            Status = SlotStatus.Available
                        });
                    }
                }

                startTime = startTime.Add(new TimeSpan(1, 0, 0));
            }

            schedule.TimeSlots = newTimeSlots;

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

            _context.TherapistSchedules.Remove(schedule);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task SyncTimeSlotStatusesAsync(DateTime currentDate)
        {
            var timeSlots = await _context.TherapistTimeSlots.ToListAsync();
            foreach (var slot in timeSlots)
            {
                var hasFutureBookings = await _context.Bookings
                    .AnyAsync(b => b.TimeSlotId == slot.TimeSlotId && b.AppointmentDate.Date >= currentDate.Date &&
                                    (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Booked));

                slot.Status = hasFutureBookings ? SlotStatus.Booked : SlotStatus.Available;
            }
            await _context.SaveChangesAsync();
        }

        private TimeSpan RoundToNextHour(TimeSpan time)
        {
            if (time.Minutes > 0)
            {
                return new TimeSpan(time.Hours + 1, 0, 0);
            }

            return time;
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
                        TimeSlotId = slot.TimeSlotId,
                        TimeSlotDescription = slot.TimeSlot.Description,
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
                        TimeSlotId = slot.TimeSlotId,
                        TimeSlotDescription = slot.TimeSlot.Description,
                        Status = slot.Status
                    }).ToList()
                }).ToListAsync();
        }

        public async Task<TherapistScheduleDTO> GetScheduleByTherapistIdAsync(int therapistId)
        {
            var schedule = await _context.TherapistSchedules
                .Include(ts => ts.TherapistUser)
                .Include(ts => ts.TimeSlots)
                .ThenInclude(ts => ts.TimeSlot)
                .FirstOrDefaultAsync(ts => ts.TherapistId == therapistId);

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
                    TimeSlotId = slot.TimeSlotId,
                    TimeSlotDescription = slot.TimeSlot.Description,
                    Status = slot.Status
                }).ToList()
            };
        }

        async Task<IEnumerable<TherapistScheduleDTO>> ITherapistScheduleService.GetScheduleByTherapistIdAsync(int therapistId)
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
                        TimeSlotId = slot.TimeSlotId,
                        TimeSlotDescription = slot.TimeSlot.Description,
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

                    // Combine AppointmentDate with TimeSlot's EndTime
                    var endTimeOnDate = booking.AppointmentDate.Date
                        .Add(booking.TherapistTimeSlot.TimeSlot.EndTime); // Directly add the TimeSpan

                    // Add 1 hour to the end time
                    var resetTime = endTimeOnDate.AddHours(1);

                    // Check if current time is past the reset time
                    if (currentTime >= resetTime)
                    {
                        booking.Status = BookingStatus.Completed;
                        booking.TherapistTimeSlot.Status = SlotStatus.Available;

                        // Mark properties as modified
                        _context.Entry(booking).Property(b => b.Status).IsModified = true;
                        _context.Entry(booking.TherapistTimeSlot).Property(ts => ts.Status).IsModified = true;

                        Console.WriteLine($"Reset Booking {booking.BookingId}: Slot {booking.TimeSlotId} set to Available. EndTime: {endTimeOnDate}, ResetTime: {resetTime}");
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
    }
}