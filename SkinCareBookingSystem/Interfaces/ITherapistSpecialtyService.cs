using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Interfaces
{
    public interface ITherapistSpecialtyService
    {
        Task<IEnumerable<TherapistSpecialtyDTO>> GetAllTherapistSpecialtiesAsync();
        Task<IEnumerable<TherapistSpecialtyDTO>> GetTherapistSpecialtiesByTherapistIdAsync(int therapistId);
        Task<IEnumerable<UserDTO>> GetTherapistsByServiceCategoryIdAsync(int serviceCategoryId);
        Task<TherapistSpecialtyDTO> CreateTherapistSpecialtyAsync(TherapistSpecialtyDTO specialtyDTO);
        Task<bool> DeleteTherapistSpecialtyAsync(int id);
        Task<TherapistSpecialtyDTO> UpdateTherapistSpecialtyAsync(int therapistId, int serviceCategoryId);
        Task<IEnumerable<ServiceDTO>> GetServicesByTherapistIdAsync(int therapistId);
    }
}
