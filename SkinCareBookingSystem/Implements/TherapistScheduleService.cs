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

        public async Task<bool> BookTimeSlotAsync(int timeSlotId, int userId, DateTime appointmentDate)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                Console.WriteLine($"Checking TimeSlotId: {timeSlotId}, Date: {appointmentDate}");

                var timeSlot = await _context.TherapistTimeSlots
                    .Include(ts => ts.TherapistSchedule)
                    .FirstOrDefaultAsync(ts => ts.TimeSlotId == timeSlotId);

                if (timeSlot == null)
                    throw new InvalidOperationException("Time slot does not exist.");

                var expectedDayOfWeek = appointmentDate.DayOfWeek;
                Console.WriteLine($"Expected DayOfWeek: {expectedDayOfWeek}, Schedule DayOfWeek: {timeSlot.TherapistSchedule.DayOfWeek}");
                if (timeSlot.TherapistSchedule.DayOfWeek != expectedDayOfWeek)
                    throw new InvalidOperationException($"Time slot is not available on {expectedDayOfWeek}. It is only available on {timeSlot.TherapistSchedule.DayOfWeek}.");

                var existingBooking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.AppointmentDate.Date == appointmentDate.Date && b.TimeSlotId == timeSlotId);

                if (existingBooking != null)
                {
                    timeSlot.Status = SlotStatus.Booked;
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"Found existing booking for TimeSlotId: {timeSlotId}, Date: {appointmentDate}");
                    throw new InvalidOperationException("Time slot is already booked for this date.");
                }

                var hasOtherBookings = await _context.Bookings
                    .AnyAsync(b => b.TimeSlotId == timeSlotId && b.AppointmentDate.Date != appointmentDate.Date);

                if (!hasOtherBookings)
                {
                    timeSlot.Status = SlotStatus.Available;
                }

                if (timeSlot.Status != SlotStatus.Available)
                {
                    Console.WriteLine($"Warning: TimeSlotId {timeSlotId} has Status {timeSlot.Status}, but no booking exists for {appointmentDate}. Proceeding with booking.");
                }

                timeSlot.Status = SlotStatus.InProcess;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
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
                    .AnyAsync(b => b.TimeSlotId == slot.TimeSlotId && b.AppointmentDate.Date >= currentDate.Date);

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
    }
}