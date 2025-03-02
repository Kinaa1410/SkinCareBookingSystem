using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.Data;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;
using SkinCareBookingSystem.Models;

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
                .Select(ts => new TherapistTimeSlotDTO
                {
                    TimeSlotId = ts.TimeSlotId,
                    ScheduleId = ts.ScheduleId,
                    StartTime = ts.StartTime,
                    EndTime = ts.EndTime,
                    IsAvailable = ts.IsAvailable
                })
                .ToListAsync();
        }

        public async Task<TherapistTimeSlotDTO> GetTimeSlotByIdAsync(int timeSlotId)
        {
            var timeSlot = await _context.TherapistTimeSlots.FindAsync(timeSlotId);
            if (timeSlot == null) return null;

            return new TherapistTimeSlotDTO
            {
                TimeSlotId = timeSlot.TimeSlotId,
                ScheduleId = timeSlot.ScheduleId,
                StartTime = timeSlot.StartTime,
                EndTime = timeSlot.EndTime,
                IsAvailable = timeSlot.IsAvailable
            };
        }

        public async Task<TherapistTimeSlotDTO> CreateTimeSlotAsync(int scheduleId, CreateTherapistTimeSlotDTO timeSlotDTO)
        {
            var scheduleExists = await _context.TherapistSchedules.AnyAsync(s => s.ScheduleId == scheduleId);
            if (!scheduleExists) throw new InvalidOperationException("Schedule does not exist.");

            var timeSlot = new TherapistTimeSlot
            {
                ScheduleId = scheduleId,
                StartTime = timeSlotDTO.StartTime,
                EndTime = timeSlotDTO.EndTime,
                IsAvailable = timeSlotDTO.IsAvailable
            };

            _context.TherapistTimeSlots.Add(timeSlot);
            await _context.SaveChangesAsync();

            return await GetTimeSlotByIdAsync(timeSlot.TimeSlotId);
        }

        public async Task<bool> UpdateTimeSlotAsync(int timeSlotId, UpdateTherapistTimeSlotDTO timeSlotDTO)
        {
            var timeSlot = await _context.TherapistTimeSlots.FindAsync(timeSlotId);
            if (timeSlot == null) return false;

            timeSlot.StartTime = timeSlotDTO.StartTime;
            timeSlot.EndTime = timeSlotDTO.EndTime;
            timeSlot.IsAvailable = timeSlotDTO.IsAvailable;

            _context.Entry(timeSlot).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteTimeSlotAsync(int timeSlotId)
        {
            var timeSlot = await _context.TherapistTimeSlots.FindAsync(timeSlotId);
            if (timeSlot == null) return false;

            _context.TherapistTimeSlots.Remove(timeSlot);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
