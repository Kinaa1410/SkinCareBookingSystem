using SkinCareBookingSystem.DTOs;

public interface ITherapistScheduleService
{
    Task<IEnumerable<TherapistScheduleDTO>> GetAllSchedulesAsync();
    Task<TherapistScheduleDTO> GetScheduleByIdAsync(int scheduleId);
    Task<TherapistScheduleDTO> CreateScheduleAsync(CreateTherapistScheduleDTO scheduleDTO);
    Task<bool> UpdateScheduleAsync(int scheduleId, UpdateTherapistScheduleDTO scheduleDTO);
    Task<bool> DeleteScheduleAsync(int scheduleId);
    Task<IEnumerable<TherapistScheduleDTO>> GetTherapistsWorkingInTimeRangeAsync(TimeSpan startTime, TimeSpan endTime);
    Task<IEnumerable<TherapistScheduleDTO>> GetTherapistsWorkingOnDayInTimeRangeAsync(DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime);
    Task<IEnumerable<TherapistScheduleDTO>> GetScheduleByTherapistIdAsync(int therapistId);

}
