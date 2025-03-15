using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.Data;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;
using SkinCareBookingSystem.Models;
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
                        IsBooked = slot.IsAvailable
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
                    IsBooked = slot.IsAvailable
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
                        IsAvailable = false // Set initial availability
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

        private TimeSpan RoundToNextHour(TimeSpan time)
        {
            // If the minutes part is greater than 0, round up to the next hour
            if (time.Minutes > 0)
            {
                return new TimeSpan(time.Hours + 1, 0, 0); // Round up to the next hour
            }

            // If already at the top of the hour, no rounding needed
            return time;
        }


        // Update a therapist's schedule, including time slots
        public async Task<bool> UpdateScheduleAsync(int scheduleId, UpdateTherapistScheduleDTO scheduleDTO)
        {
            var schedule = await _context.TherapistSchedules
                .Include(ts => ts.TimeSlots)
                .ThenInclude(ts => ts.TimeSlot) // Include TimeSlot details (StartTime, EndTime)
                .FirstOrDefaultAsync(ts => ts.ScheduleId == scheduleId);

            if (schedule == null) return false;

            // Step 1: Check if the DayOfWeek is changing
            bool isDayOfWeekChanged = schedule.DayOfWeek != scheduleDTO.DayOfWeek;

            // Step 2: Update the working times
            schedule.DayOfWeek = scheduleDTO.DayOfWeek;
            schedule.StartWorkingTime = TimeSpan.Parse(scheduleDTO.StartTime);
            schedule.EndWorkingTime = TimeSpan.Parse(scheduleDTO.EndTime);

            // Step 3: Remove old time slots if the day is changed or hours are adjusted
            var newTimeSlots = new List<TherapistTimeSlot>();

            // If the day is changing, we remove old time slots for the previous day.
            if (isDayOfWeekChanged)
            {
                var oldTimeSlotsToRemove = schedule.TimeSlots.ToList();
                _context.TherapistTimeSlots.RemoveRange(oldTimeSlotsToRemove);
            }

            // Step 4: Generate the new slots based on the new working hours
            var startTime = schedule.StartWorkingTime;
            var endTime = schedule.EndWorkingTime;

            while (startTime < endTime)
            {
                var timeSlot = await _context.TimeSlots
                    .FirstOrDefaultAsync(ts => ts.StartTime == startTime && ts.EndTime == startTime.Add(new TimeSpan(1, 0, 0))); // Default 1-hour slots

                if (timeSlot != null)
                {
                    // Check if the slot already exists for this schedule and is not booked
                    var existingSlot = schedule.TimeSlots.FirstOrDefault(ts => ts.TimeSlotId == timeSlot.TimeSlotId);
                    if (existingSlot != null)
                    {
                        // Retain the existing slot, keeping the IsBooked status
                        newTimeSlots.Add(new TherapistTimeSlot
                        {
                            TimeSlotId = timeSlot.TimeSlotId,
                            IsAvailable = existingSlot.IsAvailable // Keep existing booking status
                        });
                    }
                    else
                    {
                        // Add new slot with IsBooked = false
                        newTimeSlots.Add(new TherapistTimeSlot
                        {
                            TimeSlotId = timeSlot.TimeSlotId,
                            IsAvailable = false // New slot is available
                        });
                    }
                }

                startTime = startTime.Add(new TimeSpan(1, 0, 0)); // Add 1 hour for each slot
            }

            // Step 5: Add the newly generated time slots to the schedule
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

        // Get therapists working in a given time range
        public async Task<IEnumerable<TherapistScheduleDTO>> GetTherapistsWorkingInTimeRangeAsync(TimeSpan startTime, TimeSpan endTime)
        {
            return await _context.TherapistTimeSlots
                .Include(ts => ts.TherapistSchedule)
                .ThenInclude(s => s.TherapistUser)
                .Include(ts => ts.TimeSlot) // Include related TimeSlot
                .Where(ts => ts.TimeSlot.StartTime >= startTime && ts.TimeSlot.EndTime <= endTime && !ts.IsAvailable) // Check availability
                .Select(ts => new TherapistScheduleDTO
                {
                    ScheduleId = ts.ScheduleId,
                    TherapistId = ts.TherapistSchedule.TherapistId,
                    TherapistName = ts.TherapistSchedule.TherapistUser.UserName,
                    DayOfWeek = ts.TherapistSchedule.DayOfWeek,
                    StartWorkingTime = ts.TherapistSchedule.StartWorkingTime,
                    EndWorkingTime = ts.TherapistSchedule.EndWorkingTime,
                    TimeSlots = new List<TherapistTimeSlotDTO>
                    {
                        new TherapistTimeSlotDTO
                        {
                            TimeSlotId = ts.TimeSlotId,
                            TimeSlotDescription = ts.TimeSlot.Description,
                            IsBooked = ts.IsAvailable
                        }
                    }
                }).ToListAsync();
        }

        // Get therapists working on a given day in a time range
        public async Task<IEnumerable<TherapistScheduleDTO>> GetTherapistsWorkingOnDayInTimeRangeAsync(DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime)
        {
            return await _context.TherapistSchedules
                .Include(ts => ts.TherapistUser)
                .Include(ts => ts.TimeSlots)
                .ThenInclude(ts => ts.TimeSlot)
                .Where(ts => ts.DayOfWeek == dayOfWeek && ts.TimeSlots.Any(slot => slot.TimeSlot.StartTime >= startTime && slot.TimeSlot.EndTime <= endTime))
                .Select(ts => new TherapistScheduleDTO
                {
                    ScheduleId = ts.ScheduleId,
                    TherapistId = ts.TherapistId,
                    TherapistName = ts.TherapistUser.UserName,
                    DayOfWeek = ts.DayOfWeek,
                    StartWorkingTime = ts.StartWorkingTime,
                    EndWorkingTime = ts.EndWorkingTime,
                    TimeSlots = ts.TimeSlots.Where(slot => slot.TimeSlot.StartTime >= startTime && slot.TimeSlot.EndTime <= endTime)
                                            .Select(slot => new TherapistTimeSlotDTO
                                            {
                                                TimeSlotId = slot.TimeSlotId,
                                                TimeSlotDescription = slot.TimeSlot.Description,
                                                IsBooked = slot.IsAvailable
                                            }).ToList()
                }).ToListAsync();
        }

        // Get schedules by therapist ID
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
                    StartWorkingTime = ts.StartWorkingTime,
                    EndWorkingTime = ts.EndWorkingTime,
                    TimeSlots = ts.TimeSlots.Select(slot => new TherapistTimeSlotDTO
                    {
                        TimeSlotId = slot.TimeSlotId,
                        TimeSlotDescription = slot.TimeSlot.Description,
                        IsBooked = slot.IsAvailable
                    }).ToList()
                }).ToListAsync();
        }
    }
}
