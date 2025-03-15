using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.Data;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;
using SkinCareBookingSystem.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Implements
{
    public class TimeSlotService : ITimeSlotService
    {
        private readonly BookingDbContext _context;

        public TimeSlotService(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TimeSlotDTO>> GetAllTimeSlotsAsync()
        {
            return await _context.TimeSlots
                .Select(ts => new TimeSlotDTO
                {
                    TimeSlotId = ts.TimeSlotId,
                    StartTime = ts.StartTime,
                    EndTime = ts.EndTime
                })
                .ToListAsync();
        }

        public async Task<TimeSlotDTO> GetTimeSlotByIdAsync(int timeSlotId)
        {
            var timeSlot = await _context.TimeSlots
                .FirstOrDefaultAsync(ts => ts.TimeSlotId == timeSlotId);

            if (timeSlot == null)
                return null;

            return new TimeSlotDTO
            {
                TimeSlotId = timeSlot.TimeSlotId,
                StartTime = timeSlot.StartTime,
                EndTime = timeSlot.EndTime
            };
        }

        public async Task<TimeSlotDTO> CreateTimeSlotAsync(CreateTimeSlotDTO timeSlotDTO)
        {
            // Check if a time slot with the same StartTime and EndTime already exists in the database
            var existingTimeSlot = await _context.TimeSlots
                .FirstOrDefaultAsync(ts => ts.StartTime == timeSlotDTO.StartTime && ts.EndTime == timeSlotDTO.EndTime);

            // If a duplicate time slot is found, return an error message or throw an exception
            if (existingTimeSlot != null)
            {
                throw new InvalidOperationException("A time slot with the same start and end time already exists.");
            }

            // If no duplicate is found, create and save the new time slot
            var timeSlot = new TimeSlot
            {
                StartTime = timeSlotDTO.StartTime,
                EndTime = timeSlotDTO.EndTime
            };

            _context.TimeSlots.Add(timeSlot);
            await _context.SaveChangesAsync();

            return new TimeSlotDTO
            {
                TimeSlotId = timeSlot.TimeSlotId,
                StartTime = timeSlot.StartTime,
                EndTime = timeSlot.EndTime
            };
        }


        public async Task<bool> UpdateTimeSlotAsync(int timeSlotId, UpdateTimeSlotDTO timeSlotDTO)
        {
            var timeSlot = await _context.TimeSlots.FindAsync(timeSlotId);

            if (timeSlot == null)
                return false;

            timeSlot.StartTime = timeSlotDTO.StartTime;
            timeSlot.EndTime = timeSlotDTO.EndTime;

            _context.Entry(timeSlot).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteTimeSlotAsync(int timeSlotId)
        {
            var timeSlot = await _context.TimeSlots.FindAsync(timeSlotId);

            if (timeSlot == null)
                return false;

            _context.TimeSlots.Remove(timeSlot);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
