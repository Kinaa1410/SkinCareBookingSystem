using SkinCareBookingSystem.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SkinCareBookingSystem.Interfaces
{
    public interface ITimeSlotService
    {
        Task<IEnumerable<TimeSlotDTO>> GetAllTimeSlotsAsync();
        Task<TimeSlotDTO> GetTimeSlotByIdAsync(int timeSlotId);
        Task<TimeSlotDTO> CreateTimeSlotAsync(CreateTimeSlotDTO timeSlotDTO);
        Task<bool> UpdateTimeSlotAsync(int timeSlotId, UpdateTimeSlotDTO timeSlotDTO);
        Task<bool> DeleteTimeSlotAsync(int timeSlotId);
    }
}
