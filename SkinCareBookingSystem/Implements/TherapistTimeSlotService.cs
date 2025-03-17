using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.Data;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;
using SkinCareBookingSystem.Models;
using SkinCareBookingSystem.Enums;
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

        // Get all therapist time slots
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

        // Get a therapist time slot by ID
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

        // Create time slots for a therapist based on the schedule
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
                // Check if time slot is within the therapist's working hours
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
                        TimeSlotDescription = timeSlot.Description
                    });
                }
            }

            await _context.SaveChangesAsync();

            return newTimeSlots;
        }

        // Update the status of a time slot (for booking or availability)
        public async Task<bool> UpdateTimeSlotAsync(int timeSlotId, SlotStatus status)
        {
            var timeSlot = await _context.TherapistTimeSlots
                .FirstOrDefaultAsync(ts => ts.Id == timeSlotId);

            if (timeSlot == null)
                return false;

            // Update the status of the time slot
            timeSlot.Status = status;

            _context.Entry(timeSlot).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return true;
        }

        // Delete a therapist's time slot
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
    }
}
