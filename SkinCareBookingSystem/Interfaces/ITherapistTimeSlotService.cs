using SkinCareBookingSystem.DTOs;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Interfaces
{
    public interface ITherapistTimeSlotService
    {
        Task<IEnumerable<TherapistTimeSlotDTO>> GetAllTimeSlotsAsync();
        Task<TherapistTimeSlotDTO> GetTimeSlotByIdAsync(int timeSlotId);
        Task<TherapistTimeSlotDTO> CreateTimeSlotForTherapistAsync(int scheduleId);
        Task<bool> UpdateTimeSlotAsync(int timeSlotId, bool isBooked);
        Task<bool> DeleteTimeSlotAsync(int timeSlotId);
    }
}
