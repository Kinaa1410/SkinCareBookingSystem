using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.Data;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;
using SkinCareBookingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
                .Select(ts => new TherapistScheduleDTO
                {
                    ScheduleId = ts.ScheduleId,
                    TherapistId = ts.TherapistId,
                    TherapistName = ts.TherapistUser.UserName,
                    DayOfWeek = ts.DayOfWeek,
                    TimeSlots = ts.TimeSlots.Select(slot => new TherapistTimeSlotDTO
                    {
                        TimeSlotId = slot.TimeSlotId,
                        StartTime = slot.StartTime,
                        EndTime = slot.EndTime,
                        IsAvailable = slot.IsAvailable
                    }).ToList()
                }).ToListAsync();
        }

        public async Task<TherapistScheduleDTO> GetScheduleByIdAsync(int scheduleId)
        {
            var schedule = await _context.TherapistSchedules
                .Include(ts => ts.TherapistUser)
                .Include(ts => ts.TimeSlots)
                .FirstOrDefaultAsync(ts => ts.ScheduleId == scheduleId);

            if (schedule == null) return null;

            return new TherapistScheduleDTO
            {
                ScheduleId = schedule.ScheduleId,
                TherapistId = schedule.TherapistId,
                TherapistName = schedule.TherapistUser.UserName,
                DayOfWeek = schedule.DayOfWeek,
                TimeSlots = schedule.TimeSlots.Select(slot => new TherapistTimeSlotDTO
                {
                    TimeSlotId = slot.TimeSlotId,
                    StartTime = slot.StartTime,
                    EndTime = slot.EndTime,
                    IsAvailable = slot.IsAvailable
                }).ToList()
            };
        }

        public async Task<TherapistScheduleDTO> CreateScheduleAsync(CreateTherapistScheduleDTO scheduleDTO)
        {
            var schedule = new TherapistSchedule
            {
                TherapistId = scheduleDTO.TherapistId,
                DayOfWeek = scheduleDTO.DayOfWeek,
                TimeSlots = scheduleDTO.TimeSlots.Select(slot => new TherapistTimeSlot
                {
                    StartTime = slot.StartTime,
                    EndTime = slot.EndTime,
                    IsAvailable = slot.IsAvailable
                }).ToList()
            };

            _context.TherapistSchedules.Add(schedule);
            await _context.SaveChangesAsync();

            return await GetScheduleByIdAsync(schedule.ScheduleId);
        }

        public async Task<bool> UpdateScheduleAsync(int scheduleId, UpdateTherapistScheduleDTO scheduleDTO)
        {
            var schedule = await _context.TherapistSchedules
                .Include(ts => ts.TimeSlots)
                .FirstOrDefaultAsync(ts => ts.ScheduleId == scheduleId);

            if (schedule == null) return false;

            schedule.DayOfWeek = scheduleDTO.DayOfWeek;
            schedule.TimeSlots = scheduleDTO.TimeSlots.Select(slot => new TherapistTimeSlot
            {
                TimeSlotId = slot.TimeSlotId,
                StartTime = slot.StartTime,
                EndTime = slot.EndTime,
                IsAvailable = slot.IsAvailable
            }).ToList();

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

        public async Task<IEnumerable<TherapistScheduleDTO>> GetTherapistsWorkingInTimeRangeAsync(TimeSpan startTime, TimeSpan endTime)
        {
            return await _context.TherapistTimeSlots
                .Include(ts => ts.TherapistSchedule)
                .ThenInclude(s => s.TherapistUser)
                .Where(ts => ts.StartTime >= startTime && ts.EndTime <= endTime && ts.IsAvailable)
                .Select(ts => new TherapistScheduleDTO
                {
                    ScheduleId = ts.ScheduleId,
                    TherapistId = ts.TherapistSchedule.TherapistId,
                    TherapistName = ts.TherapistSchedule.TherapistUser.UserName,
                    DayOfWeek = ts.TherapistSchedule.DayOfWeek,
                    TimeSlots = new List<TherapistTimeSlotDTO>
                    {
                        new TherapistTimeSlotDTO
                        {
                            TimeSlotId = ts.TimeSlotId,
                            StartTime = ts.StartTime,
                            EndTime = ts.EndTime,
                            IsAvailable = ts.IsAvailable
                        }
                    }
                }).ToListAsync();
        }

        public async Task<IEnumerable<TherapistScheduleDTO>> GetScheduleByTherapistIdAsync(int therapistId)
        {
            return await _context.TherapistSchedules
                .Include(ts => ts.TherapistUser)
                .Include(ts => ts.TimeSlots)
                .Where(ts => ts.TherapistId == therapistId)
                .Select(ts => new TherapistScheduleDTO
                {
                    ScheduleId = ts.ScheduleId,
                    TherapistId = ts.TherapistId,
                    TherapistName = ts.TherapistUser.UserName,
                    DayOfWeek = ts.DayOfWeek,
                    TimeSlots = ts.TimeSlots.Select(slot => new TherapistTimeSlotDTO
                    {
                        TimeSlotId = slot.TimeSlotId,
                        StartTime = slot.StartTime,
                        EndTime = slot.EndTime,
                        IsAvailable = slot.IsAvailable
                    }).ToList()
                }).ToListAsync();
        }

        public async Task<IEnumerable<TherapistScheduleDTO>> GetTherapistsWorkingOnDayInTimeRangeAsync(DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime)
        {
            return await _context.TherapistSchedules
                .Include(ts => ts.TherapistUser)
                .Include(ts => ts.TimeSlots)
                .Where(ts => ts.DayOfWeek == dayOfWeek && ts.TimeSlots.Any(slot => slot.StartTime >= startTime && slot.EndTime <= endTime))
                .Select(ts => new TherapistScheduleDTO
                {
                    ScheduleId = ts.ScheduleId,
                    TherapistId = ts.TherapistId,
                    TherapistName = ts.TherapistUser.UserName,
                    DayOfWeek = ts.DayOfWeek,
                    TimeSlots = ts.TimeSlots.Where(slot => slot.StartTime >= startTime && slot.EndTime <= endTime)
                                            .Select(slot => new TherapistTimeSlotDTO
                                            {
                                                TimeSlotId = slot.TimeSlotId,
                                                StartTime = slot.StartTime,
                                                EndTime = slot.EndTime,
                                                IsAvailable = slot.IsAvailable
                                            }).ToList()
                })
                .ToListAsync();
        }
    }
}
