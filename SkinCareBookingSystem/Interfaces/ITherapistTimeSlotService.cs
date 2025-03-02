using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Interfaces
{
    public interface ITherapistTimeSlotService
    {
        Task<IEnumerable<TherapistTimeSlotDTO>> GetAllTimeSlotsAsync();
        Task<TherapistTimeSlotDTO> GetTimeSlotByIdAsync(int timeSlotId);
        Task<TherapistTimeSlotDTO> CreateTimeSlotAsync(int scheduleId, CreateTherapistTimeSlotDTO timeSlotDTO);
        Task<bool> UpdateTimeSlotAsync(int timeSlotId, UpdateTherapistTimeSlotDTO timeSlotDTO);
        Task<bool> DeleteTimeSlotAsync(int timeSlotId);
    }
}
