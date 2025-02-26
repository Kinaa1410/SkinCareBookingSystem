using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.Data;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;
using SkinCareBookingSystem.Models;

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
                .Select(ts => new TherapistScheduleDTO
                {
                    ScheduleId = ts.ScheduleId,
                    TherapistId = ts.TherapistId,
                    TherapistName = ts.TherapistUser.UserName,
                    DayOfWeek = ts.DayOfWeek,
                    StartTime = ts.StartTime,
                    EndTime = ts.EndTime,
                    IsAvailable = ts.IsAvailable
                }).ToListAsync();
        }

        public async Task<TherapistScheduleDTO> GetScheduleByIdAsync(int scheduleId)
        {
            var schedule = await _context.TherapistSchedules
                .Include(ts => ts.TherapistUser)
                .FirstOrDefaultAsync(ts => ts.ScheduleId == scheduleId);

            if (schedule == null) return null;

            return new TherapistScheduleDTO
            {
                ScheduleId = schedule.ScheduleId,
                TherapistId = schedule.TherapistId,
                TherapistName = schedule.TherapistUser.UserName,
                DayOfWeek = schedule.DayOfWeek,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                IsAvailable = schedule.IsAvailable
            };
        }

        public async Task<TherapistScheduleDTO> CreateScheduleAsync(CreateTherapistScheduleDTO scheduleDTO)
        {
            var startTime = new TimeSpan(scheduleDTO.StartHour, scheduleDTO.StartMinute, 0);
            var endTime = new TimeSpan(scheduleDTO.EndHour, scheduleDTO.EndMinute, 0);

            var schedule = new TherapistSchedule
            {
                TherapistId = scheduleDTO.TherapistId,
                DayOfWeek = scheduleDTO.DayOfWeek,
                StartTime = startTime,
                EndTime = endTime,
                IsAvailable = scheduleDTO.IsAvailable
            };

            _context.TherapistSchedules.Add(schedule);
            await _context.SaveChangesAsync();

            return new TherapistScheduleDTO
            {
                ScheduleId = schedule.ScheduleId,
                TherapistId = schedule.TherapistId,
                DayOfWeek = schedule.DayOfWeek,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                IsAvailable = schedule.IsAvailable
            };
        }

        public async Task<bool> UpdateScheduleAsync(int scheduleId, UpdateTherapistScheduleDTO scheduleDTO)
        {
            var schedule = await _context.TherapistSchedules.FindAsync(scheduleId);
            if (schedule == null) return false;

            schedule.DayOfWeek = scheduleDTO.DayOfWeek;
            schedule.StartTime = new TimeSpan(scheduleDTO.StartHour, scheduleDTO.StartMinute, 0);
            schedule.EndTime = new TimeSpan(scheduleDTO.EndHour, scheduleDTO.EndMinute, 0);
            schedule.IsAvailable = scheduleDTO.IsAvailable;

            _context.Entry(schedule).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteScheduleAsync(int scheduleId)
        {
            var schedule = await _context.TherapistSchedules.FindAsync(scheduleId);
            if (schedule == null) return false;

            _context.TherapistSchedules.Remove(schedule);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<TherapistScheduleDTO>> GetTherapistsWorkingOnDayAsync(DayOfWeek dayOfWeek)
        {
            var schedules = await _context.TherapistSchedules
                .Include(ts => ts.TherapistUser)
                .Where(ts => ts.DayOfWeek == dayOfWeek && ts.IsAvailable)
                .Select(ts => new TherapistScheduleDTO
                {
                    ScheduleId = ts.ScheduleId,
                    TherapistId = ts.TherapistId,
                    TherapistName = ts.TherapistUser.UserName,
                    DayOfWeek = ts.DayOfWeek,
                    StartTime = ts.StartTime,
                    EndTime = ts.EndTime,
                    IsAvailable = ts.IsAvailable
                })
                .ToListAsync();

            return schedules;
        }

        public async Task<IEnumerable<TherapistScheduleDTO>> GetTherapistsWorkingInTimeRangeAsync(TimeSpan startTime, TimeSpan endTime)
        {
            var schedules = await _context.TherapistSchedules
                .Include(ts => ts.TherapistUser)
                .Where(ts => ts.StartTime >= startTime && ts.EndTime <= endTime && ts.IsAvailable)
                .Select(ts => new TherapistScheduleDTO
                {
                    ScheduleId = ts.ScheduleId,
                    TherapistId = ts.TherapistId,
                    TherapistName = ts.TherapistUser.UserName,
                    DayOfWeek = ts.DayOfWeek,
                    StartTime = ts.StartTime,
                    EndTime = ts.EndTime,
                    IsAvailable = ts.IsAvailable
                })
                .ToListAsync();

            return schedules;
        }

        public async Task<IEnumerable<TherapistScheduleDTO>> GetTherapistsWorkingOnDayInTimeRangeAsync(DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime)
        {
            var schedules = await _context.TherapistSchedules
                .Include(ts => ts.TherapistUser)
                .Where(ts => ts.DayOfWeek == dayOfWeek
                            && ts.IsAvailable
                            && ts.StartTime >= startTime
                            && ts.EndTime <= endTime)
                .Select(ts => new TherapistScheduleDTO
                {
                    ScheduleId = ts.ScheduleId,
                    TherapistId = ts.TherapistId,
                    TherapistName = ts.TherapistUser.UserName,
                    DayOfWeek = ts.DayOfWeek,
                    StartTime = ts.StartTime,
                    EndTime = ts.EndTime,
                    IsAvailable = ts.IsAvailable
                })
                .ToListAsync();

            return schedules;
        }

        public async Task<IEnumerable<TherapistScheduleDTO>> GetScheduleByTherapistIdAsync(int therapistId)
        {
            var schedules = await _context.TherapistSchedules
                .Include(ts => ts.TherapistUser)
                .Where(ts => ts.TherapistId == therapistId && ts.IsAvailable)
                .Select(ts => new TherapistScheduleDTO
                {
                    ScheduleId = ts.ScheduleId,
                    TherapistId = ts.TherapistId,
                    TherapistName = ts.TherapistUser.UserName,
                    DayOfWeek = ts.DayOfWeek,
                    StartTime = ts.StartTime,
                    EndTime = ts.EndTime,
                    IsAvailable = ts.IsAvailable
                })
                .ToListAsync();

            return schedules;
        }

    }
}
