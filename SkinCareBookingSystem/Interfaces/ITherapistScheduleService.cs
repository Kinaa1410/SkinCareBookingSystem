using SkinCareBookingSystem.DTOs;
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
        Task<bool> BookTimeSlotAsync(int therapistTimeSlotId, int userId, DateTime appointmentDate, int therapistId);
        Task<bool> UpdateScheduleAsync(int scheduleId, UpdateTherapistScheduleDTO scheduleDTO);
        Task<bool> DeleteScheduleAsync(int scheduleId);
        Task SyncTimeSlotStatusesAsync(DateTime currentDate);
        Task<IEnumerable<TherapistScheduleDTO>> GetTherapistsWorkingOnDayInTimeRangeAsync(DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime);
        Task<IEnumerable<TherapistScheduleDTO>> GetTherapistsWorkingInTimeRangeAsync(TimeSpan startTime, TimeSpan endTime);
        Task<IEnumerable<TherapistScheduleDTO>> GetScheduleByTherapistIdAsync(int therapistId);
        Task ResetCompletedBookingsAsync();
        Task<List<TherapistTimeSlotDTO>> GetAvailableSlotsAsync(int therapistId, DateTime date);
        Task MarkSlotPermanentlyUnavailableAsync(int therapistTimeSlotId);
        Task<TherapistTimeSlotDTO> GetTherapistTimeSlotByIdAsync(int therapistTimeSlotId);
        Task<TherapistTimeSlotDTO> CreateTherapistTimeSlotAsync(CreateTherapistTimeSlotDTO timeSlotDTO);
        Task<bool> UpdateTherapistTimeSlotAsync(int therapistTimeSlotId, UpdateTherapistTimeSlotDTO timeSlotDTO);
        Task<bool> DeleteTherapistTimeSlotAsync(int therapistTimeSlotId);
    }
}