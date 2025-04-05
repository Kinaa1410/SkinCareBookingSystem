using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.Data;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;
using SkinCareBookingSystem.Models;
using SkinCareBookingSystem.Enums;
using System.Linq;
using System.Threading.Tasks;
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
                    TimeSlotDescription = $"{ts.TimeSlot.StartTime} - {ts.TimeSlot.EndTime}",
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
                TimeSlotDescription = $"{timeSlot.TimeSlot.StartTime} - {timeSlot.TimeSlot.EndTime}",
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
                    var existingSlot = await _context.TherapistTimeSlots
                        .Include(ts => ts.TimeSlot)
                        .Where(ts => ts.ScheduleId == scheduleId &&
                                     ts.TimeSlot.StartTime == timeSlot.StartTime &&
                                     ts.TimeSlot.EndTime == timeSlot.EndTime)
                        .FirstOrDefaultAsync();

                    if (existingSlot != null)
                        continue;

                    var therapistTimeSlot = new TherapistTimeSlot
                    {
                        ScheduleId = scheduleId,
                        TimeSlotId = timeSlot.TimeSlotId,
                        Status = SlotStatus.Available
                    };

                    _context.TherapistTimeSlots.Add(therapistTimeSlot);
                    await _context.SaveChangesAsync();

                    newTimeSlots.Add(new TherapistTimeSlotDTO
                    {
                        Id = therapistTimeSlot.Id,
                        ScheduleId = scheduleId,
                        TimeSlotId = timeSlot.TimeSlotId,
                        TimeSlotDescription = $"{timeSlot.StartTime} - {timeSlot.EndTime}",
                        Status = SlotStatus.Available
                    });
                }
            }

            return newTimeSlots;
        }

        public async Task<TherapistTimeSlotDTO> CreateTherapistTimeSlotAsync(CreateTherapistTimeSlotDTO timeSlotDTO)
        {
            var schedule = await _context.TherapistSchedules
                .FirstOrDefaultAsync(s => s.ScheduleId == timeSlotDTO.ScheduleId);

            if (schedule == null)
                throw new InvalidOperationException($"Schedule with ID {timeSlotDTO.ScheduleId} not found.");

            var actualTimeSlot = await _context.TimeSlots
                .FirstOrDefaultAsync(ts => ts.TimeSlotId == timeSlotDTO.TimeSlotId);

            if (actualTimeSlot == null)
                throw new InvalidOperationException($"Time slot with ID {timeSlotDTO.TimeSlotId} not found.");

            var therapistTimeSlot = new TherapistTimeSlot
            {
                ScheduleId = timeSlotDTO.ScheduleId,
                TimeSlotId = timeSlotDTO.TimeSlotId,
                Status = timeSlotDTO.Status
            };

            _context.TherapistTimeSlots.Add(therapistTimeSlot);
            await _context.SaveChangesAsync();

            return new TherapistTimeSlotDTO
            {
                Id = therapistTimeSlot.Id,
                ScheduleId = therapistTimeSlot.ScheduleId,
                TimeSlotId = therapistTimeSlot.TimeSlotId,
                TimeSlotDescription = $"{actualTimeSlot.StartTime} - {actualTimeSlot.EndTime}",
                Status = therapistTimeSlot.Status
            };
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

            // Remove associated TherapistTimeSlotLocks
            var locks = await _context.TherapistTimeSlotLocks
                .Where(tsl => tsl.TherapistTimeSlotId == timeSlotId)
                .ToListAsync();

            if (locks.Any())
            {
                _context.TherapistTimeSlotLocks.RemoveRange(locks);
            }

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
                var locks = await _context.TherapistTimeSlotLocks
                    .Where(tsl => tsl.TherapistTimeSlotId == timeSlot.Id)
                    .ToListAsync();

                foreach (var lockEntry in locks)
                {
                    var startOfLockWeek = lockEntry.Date.Date.AddDays(-(int)lockEntry.Date.DayOfWeek);
                    if (startOfLockWeek < startOfCurrentWeek && timeSlot.Status != SlotStatus.Available)
                    {
                        // Check if there are any associated bookings
                        var booking = await _context.Bookings
                            .FirstOrDefaultAsync(b => b.TherapistTimeSlotId == timeSlot.Id &&
                                                      b.AppointmentDate.Date == lockEntry.Date);

                        if (booking != null && (booking.Status == BookingStatus.Failed ||
                                                booking.Status == BookingStatus.Canceled || // Changed from Cancelled to Canceled
                                                booking.Status == BookingStatus.Completed))
                        {
                            _context.TherapistTimeSlotLocks.Remove(lockEntry);
                        }
                    }
                }

                // Update the status based on remaining locks
                var hasActiveLocks = await _context.TherapistTimeSlotLocks
                    .AnyAsync(tsl => tsl.TherapistTimeSlotId == timeSlot.Id &&
                                     (tsl.Status == SlotStatus.InProcess || tsl.Status == SlotStatus.Booked));

                timeSlot.Status = hasActiveLocks ? SlotStatus.Booked : SlotStatus.Available;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TherapistTimeSlotDTO>> GetAvailableTimeSlotsAsync()
        {
            var currentDate = DateTime.Now.Date;
            return await _context.TherapistTimeSlots
                .Include(ts => ts.TimeSlot)
                .Where(ts => ts.Status == SlotStatus.Available)
                .GroupJoin(_context.TherapistTimeSlotLocks,
                    ts => ts.Id,
                    tsl => tsl.TherapistTimeSlotId,
                    (ts, locks) => new { TherapistTimeSlot = ts, Locks = locks })
                .SelectMany(x => x.Locks.DefaultIfEmpty(), (x, tsl) => new { x.TherapistTimeSlot, Lock = tsl })
                .Where(x => x.Lock == null || x.Lock.Date != currentDate ||
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
        }

        public async Task<IEnumerable<TherapistTimeSlotDTO>> GetAvailableTimeSlotsAsync(int therapistId)
        {
            var currentDate = DateTime.Now.Date;
            return await _context.TherapistTimeSlots
                .Include(ts => ts.TimeSlot)
                .Include(ts => ts.TherapistSchedule)
                .Where(ts => ts.Status == SlotStatus.Available && ts.TherapistSchedule.TherapistId == therapistId)
                .GroupJoin(_context.TherapistTimeSlotLocks,
                    ts => ts.Id,
                    tsl => tsl.TherapistTimeSlotId,
                    (ts, locks) => new { TherapistTimeSlot = ts, Locks = locks })
                .SelectMany(x => x.Locks.DefaultIfEmpty(), (x, tsl) => new { x.TherapistTimeSlot, Lock = tsl })
                .Where(x => x.Lock == null || x.Lock.Date != currentDate ||
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
        }
    }
}