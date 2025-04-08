using SkinCareBookingSystem.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using SkinCareBookingSystem.Enums;

namespace SkinCareBookingSystem.Interfaces
{
    public interface ITherapistTimeSlotService
    {
        Task<IEnumerable<TherapistTimeSlotDTO>> GetAllTimeSlotsAsync();
        Task<TherapistTimeSlotDTO> GetTimeSlotByIdAsync(int timeSlotId);
        Task<IEnumerable<TherapistTimeSlotDTO>> CreateTimeSlotForTherapistAsync(int scheduleId);
        Task<TherapistTimeSlotDTO> CreateTherapistTimeSlotAsync(CreateTherapistTimeSlotDTO timeSlotDTO);
        Task<bool> UpdateTimeSlotAsync(int timeSlotId, SlotStatus status);
        Task<bool> DeleteTimeSlotAsync(int timeSlotId);
        Task<IEnumerable<TherapistTimeSlotDTO>> GetAvailableTimeSlotsAsync();
        Task<IEnumerable<TherapistTimeSlotDTO>> GetAvailableTimeSlotsAsync(int therapistId);
    }
}