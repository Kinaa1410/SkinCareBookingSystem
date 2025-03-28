﻿using SkinCareBookingSystem.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Interfaces
{
    public interface ITherapistScheduleService
    {
        Task<IEnumerable<TherapistScheduleDTO>> GetAllSchedulesAsync();
        Task<TherapistScheduleDTO> GetScheduleByIdAsync(int scheduleId);
        Task<TherapistScheduleDTO> CreateScheduleAsync(CreateTherapistScheduleDTO scheduleDTO);
        Task<bool> UpdateScheduleAsync(int scheduleId, UpdateTherapistScheduleDTO scheduleDTO);
        Task<bool> DeleteScheduleAsync(int scheduleId);
        Task<bool> BookTimeSlotAsync(int timeSlotId, int userId, DateTime appointmentDate, int therapistId);
        Task<IEnumerable<TherapistScheduleDTO>> GetScheduleByTherapistIdAsync(int therapistId);
        Task<IEnumerable<TherapistScheduleDTO>> GetTherapistsWorkingInTimeRangeAsync(TimeSpan startTime, TimeSpan endTime);
        Task<IEnumerable<TherapistScheduleDTO>> GetTherapistsWorkingOnDayInTimeRangeAsync(DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime);
        Task ResetCompletedBookingsAsync();
    }
}
