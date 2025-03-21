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

        // Get all therapist schedules, including time slots
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

        // Get therapist schedule by ID
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
                    Status = SlotStatus.Available
                }).ToList()
            };
        }

        // Create a new therapist schedule with time slots
        public async Task<TherapistScheduleDTO> CreateScheduleAsync(CreateTherapistScheduleDTO scheduleDTO)
        {
            var therapist = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == scheduleDTO.TherapistId && u.RoleId == 2); // Ensure the user is a therapist

            if (therapist == null)
                throw new InvalidOperationException($"Therapist with ID {scheduleDTO.TherapistId} not found.");

            // Parse the start and end times from the DTO
            var startTime = TimeSpan.Parse(scheduleDTO.StartTime);
            var endTime = TimeSpan.Parse(scheduleDTO.EndTime);

            // Round the start time and end time to the next full hour (if necessary)
            startTime = RoundToNextHour(startTime);
            endTime = RoundToNextHour(endTime);

            var timeSlots = new List<TherapistTimeSlot>();

            // Loop to create the time slots for the therapist schedule
            while (startTime < endTime)
            {
                var timeSlot = await _context.TimeSlots
                    .FirstOrDefaultAsync(ts => ts.StartTime == startTime && ts.EndTime == startTime.Add(new TimeSpan(1, 0, 0))); // Default 1-hour slots

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
                    // If no matching time slot is found, return an error
                    throw new InvalidOperationException($"Time slot for {startTime} to {startTime.Add(new TimeSpan(1, 0, 0))} does not exist.");
                }

                // Add 1 hour for each slot
                startTime = startTime.Add(new TimeSpan(1, 0, 0)); // Add 1 hour for each slot
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

        // Method to handle the booking process
        public async Task<bool> BookTimeSlotAsync(int timeSlotId, int userId)
        {
            var slot = await _context.TherapistTimeSlots
                .FirstOrDefaultAsync(ts => ts.TimeSlotId == timeSlotId && ts.Status == SlotStatus.Available);

            if (slot == null)
            {
                throw new InvalidOperationException("Time slot is either already booked or unavailable.");
            }

            slot.Status = SlotStatus.InProcess;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CompletePaymentAsync(int timeSlotId)
        {
            var slot = await _context.TherapistTimeSlots
                .FirstOrDefaultAsync(ts => ts.TimeSlotId == timeSlotId && ts.Status == SlotStatus.InProcess);

            if (slot == null)
            {
                throw new InvalidOperationException("Time slot is not in process or already booked.");
            }

            slot.Status = SlotStatus.Booked;
            await _context.SaveChangesAsync();

            return true;
        }

        // Method to update a therapist's schedule, including time slots
        public async Task<bool> UpdateScheduleAsync(int scheduleId, UpdateTherapistScheduleDTO scheduleDTO)
        {
            var schedule = await _context.TherapistSchedules
                .Include(ts => ts.TimeSlots)
                .ThenInclude(ts => ts.TimeSlot)
                .FirstOrDefaultAsync(ts => ts.ScheduleId == scheduleId);

            if (schedule == null) return false;

            // Check if the DayOfWeek is changing
            bool isDayOfWeekChanged = schedule.DayOfWeek != scheduleDTO.DayOfWeek;

            // Update working times
            schedule.DayOfWeek = scheduleDTO.DayOfWeek;
            schedule.StartWorkingTime = TimeSpan.Parse(scheduleDTO.StartTime);
            schedule.EndWorkingTime = TimeSpan.Parse(scheduleDTO.EndTime);

            var newTimeSlots = new List<TherapistTimeSlot>();

            // Remove old time slots if the day is changed or hours are adjusted
            if (isDayOfWeekChanged)
            {
                var oldTimeSlotsToRemove = schedule.TimeSlots.ToList();
                _context.TherapistTimeSlots.RemoveRange(oldTimeSlotsToRemove);
            }

            // Generate the new slots based on the new working hours
            var startTime = schedule.StartWorkingTime;
            var endTime = schedule.EndWorkingTime;

            while (startTime < endTime)
            {
                var timeSlot = await _context.TimeSlots
                    .FirstOrDefaultAsync(ts => ts.StartTime == startTime && ts.EndTime == startTime.Add(new TimeSpan(1, 0, 0))); // Default 1-hour slots

                if (timeSlot != null)
                {
                    var existingSlot = schedule.TimeSlots.FirstOrDefault(ts => ts.TimeSlotId == timeSlot.TimeSlotId);
                    if (existingSlot != null)
                    {
                        newTimeSlots.Add(new TherapistTimeSlot
                        {
                            TimeSlotId = timeSlot.TimeSlotId,
                            Status = existingSlot.Status // Correct usage of SlotStatus enum
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

                startTime = startTime.Add(new TimeSpan(1, 0, 0)); // Add 1 hour for each slot
            }

            schedule.TimeSlots = newTimeSlots;

            _context.Entry(schedule).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return true;
        }

        // Delete a therapist's schedule
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

        private TimeSpan RoundToNextHour(TimeSpan time)
        {
            if (time.Minutes > 0)
            {
                return new TimeSpan(time.Hours + 1, 0, 0); // Round up to the next hour
            }

            return time; // No rounding needed if already at the top of the hour
        }

        // Example implementation for missing interface methods:

        // Get therapists working on a specific day within a time range
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
                        Status = SlotStatus.Available
                    }).ToList()
                }).ToListAsync();
        }

        // Get therapists working in a specific time range
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
                        Status = SlotStatus.Available
                    }).ToList()
                }).ToListAsync();
        }

        // Get schedule by therapist id
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
                    Status = SlotStatus.Available
                }).ToList()
            };
        }

        Task<IEnumerable<TherapistScheduleDTO>> ITherapistScheduleService.GetScheduleByTherapistIdAsync(int therapistId)
        {
            throw new NotImplementedException();
        }
    }
}
