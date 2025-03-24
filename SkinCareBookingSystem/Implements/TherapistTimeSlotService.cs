using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.Data;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;
using SkinCareBookingSystem.Models;
using SkinCareBookingSystem.Enums;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace SkinCareBookingSystem.Implements
{
    public class TherapistTimeSlotService : ITherapistTimeSlotService
    {
        private readonly BookingDbContext _context;

        public TherapistTimeSlotService(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TherapistTimeSlotDTO>> GetAllTimeSlotsAsync()
        {
            return await _context.TherapistTimeSlots
                .Include(ts => ts.TimeSlot)
                .Select(ts => new TherapistTimeSlotDTO
                {
                    Id = ts.Id,
                    ScheduleId = ts.ScheduleId,
                    TimeSlotId = ts.TimeSlotId,
                    TimeSlotDescription = ts.TimeSlot.Description,
                    Status = ts.Status
                })
                .ToListAsync();
        }

        public async Task<TherapistTimeSlotDTO> GetTimeSlotByIdAsync(int timeSlotId)
        {
            var timeSlot = await _context.TherapistTimeSlots
                .Include(ts => ts.TimeSlot)
                .FirstOrDefaultAsync(ts => ts.Id == timeSlotId);

            if (timeSlot == null)
                return null;

            return new TherapistTimeSlotDTO
            {
                Id = timeSlot.Id,
                ScheduleId = timeSlot.ScheduleId,
                TimeSlotId = timeSlot.TimeSlotId,
                TimeSlotDescription = timeSlot.TimeSlot.Description,
                Status = timeSlot.Status
            };
        }

        public async Task<IEnumerable<TherapistTimeSlotDTO>> CreateTimeSlotForTherapistAsync(int scheduleId)
        {
            var schedule = await _context.TherapistSchedules
                .Include(s => s.TimeSlots)
                .FirstOrDefaultAsync(s => s.ScheduleId == scheduleId);

            if (schedule == null)
                throw new InvalidOperationException("Therapist schedule not found.");

            var timeSlots = await _context.TimeSlots.ToListAsync();
            var newTimeSlots = new List<TherapistTimeSlotDTO>();

            foreach (var timeSlot in timeSlots)
            {
                if (schedule.StartWorkingTime <= timeSlot.StartTime && schedule.EndWorkingTime >= timeSlot.EndTime)
                {
                    var therapistTimeSlot = new TherapistTimeSlot
                    {
                        ScheduleId = scheduleId,
                        TimeSlotId = timeSlot.TimeSlotId,
                        Status = SlotStatus.Available
                    };

                    _context.TherapistTimeSlots.Add(therapistTimeSlot);

                    newTimeSlots.Add(new TherapistTimeSlotDTO
                    {
                        ScheduleId = scheduleId,
                        TimeSlotId = timeSlot.TimeSlotId,
                        TimeSlotDescription = timeSlot.Description,
                        Status = SlotStatus.Available
                    });
                }
            }

            await _context.SaveChangesAsync();
            return newTimeSlots;
        }

        public async Task<bool> UpdateTimeSlotAsync(int timeSlotId, SlotStatus status)
        {
            var timeSlot = await _context.TherapistTimeSlots
                .FirstOrDefaultAsync(ts => ts.Id == timeSlotId);

            if (timeSlot == null)
                return false;

            timeSlot.Status = status;
            _context.Entry(timeSlot).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteTimeSlotAsync(int timeSlotId)
        {
            var timeSlot = await _context.TherapistTimeSlots
                .FirstOrDefaultAsync(ts => ts.Id == timeSlotId);

            if (timeSlot == null)
                return false;

            _context.TherapistTimeSlots.Remove(timeSlot);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task ResetWeeklyTimeSlotsAsync()
        {
            var currentDate = DateTime.Now;
            var startOfCurrentWeek = currentDate.Date.AddDays(-(int)currentDate.DayOfWeek);
            var timeSlots = await _context.TherapistTimeSlots
                .Include(ts => ts.TimeSlot)
                .ToListAsync();

            foreach (var timeSlot in timeSlots)
            {
                var booking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.TimeSlotId == timeSlot.Id);

                if (booking != null)
                {
                    var startOfBookingWeek = booking.AppointmentDate.Date.AddDays(-(int)booking.AppointmentDate.DayOfWeek);
                    if (startOfBookingWeek < startOfCurrentWeek && timeSlot.Status != SlotStatus.Available && !booking.Status)
                    {
                        timeSlot.Status = SlotStatus.Available;
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TherapistTimeSlotDTO>> GetAvailableTimeSlotsAsync()
        {
            return await _context.TherapistTimeSlots
                .Include(ts => ts.TimeSlot)
                .Where(ts => ts.Status == SlotStatus.Available)
                .Select(ts => new TherapistTimeSlotDTO
                {
                    Id = ts.Id,
                    ScheduleId = ts.ScheduleId,
                    TimeSlotId = ts.TimeSlotId,
                    TimeSlotDescription = ts.TimeSlot.Description,
                    Status = ts.Status
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<TherapistTimeSlotDTO>> GetAvailableTimeSlotsAsync(int therapistId)
        {
            return await _context.TherapistTimeSlots
                .Include(ts => ts.TimeSlot)
                .Include(ts => ts.TherapistSchedule)
                .Where(ts => ts.Status == SlotStatus.Available && ts.TherapistSchedule.TherapistId == therapistId)
                .Select(ts => new TherapistTimeSlotDTO
                {
                    Id = ts.Id,
                    ScheduleId = ts.ScheduleId,
                    TimeSlotId = ts.TimeSlotId,
                    TimeSlotDescription = ts.TimeSlot.Description,
                    Status = ts.Status
                })
                .ToListAsync();
        }
    }
}