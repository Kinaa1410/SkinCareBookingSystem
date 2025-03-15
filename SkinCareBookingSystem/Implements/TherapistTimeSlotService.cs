using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.Data;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;
using SkinCareBookingSystem.Models;
using System.Threading.Tasks;
using System.Linq;

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
                    IsBooked = ts.IsAvailable
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
                IsBooked = timeSlot.IsAvailable
            };
        }

        public async Task<TherapistTimeSlotDTO> CreateTimeSlotForTherapistAsync(int scheduleId)
        {
            var schedule = await _context.TherapistSchedules
                .Include(s => s.TimeSlots)
                .FirstOrDefaultAsync(s => s.ScheduleId == scheduleId);

            if (schedule == null)
                throw new InvalidOperationException("Therapist schedule not found.");

            var timeSlots = await _context.TimeSlots.ToListAsync();

            foreach (var timeSlot in timeSlots)
            {
                if (schedule.StartWorkingTime <= timeSlot.StartTime && schedule.EndWorkingTime >= timeSlot.EndTime)
                {
                    var therapistTimeSlot = new TherapistTimeSlot
                    {
                        ScheduleId = scheduleId,
                        TimeSlotId = timeSlot.TimeSlotId,
                        IsAvailable = false
                    };

                    _context.TherapistTimeSlots.Add(therapistTimeSlot);
                }
            }

            await _context.SaveChangesAsync();

            return new TherapistTimeSlotDTO
            {
                ScheduleId = scheduleId,
                TimeSlotDescription = timeSlots.First().Description // Example of the first slot
            };
        }

        public async Task<bool> UpdateTimeSlotAsync(int timeSlotId, bool isAvailable)
        {
            var timeSlot = await _context.TherapistTimeSlots.FindAsync(timeSlotId);

            if (timeSlot == null)
                return false;

            timeSlot.IsAvailable = isAvailable;

            _context.Entry(timeSlot).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteTimeSlotAsync(int timeSlotId)
        {
            var timeSlot = await _context.TherapistTimeSlots.FindAsync(timeSlotId);

            if (timeSlot == null)
                return false;

            _context.TherapistTimeSlots.Remove(timeSlot);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
