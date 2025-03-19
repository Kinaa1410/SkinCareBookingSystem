using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Enums;  // Include the SlotStatus enum
using System.Threading.Tasks;
using System.Collections.Generic;  // For IEnumerable

namespace SkinCareBookingSystem.Interfaces
{
    public interface ITherapistTimeSlotService
    {
        // Get all therapist time slots
        Task<IEnumerable<TherapistTimeSlotDTO>> GetAllTimeSlotsAsync();

        // Get a therapist time slot by its ID
        Task<TherapistTimeSlotDTO> GetTimeSlotByIdAsync(int timeSlotId);

        // Create time slots for a therapist based on their schedule
        Task<IEnumerable<TherapistTimeSlotDTO>> CreateTimeSlotForTherapistAsync(int scheduleId);

        // Update the status of a specific time slot (e.g., Available, InProcess, or Booked)
        Task<bool> UpdateTimeSlotAsync(int timeSlotId, SlotStatus status);

        // Delete a therapist's time slot
        Task<bool> DeleteTimeSlotAsync(int timeSlotId);
        Task ResetWeeklyTimeSlotsAsync();
    }
}
