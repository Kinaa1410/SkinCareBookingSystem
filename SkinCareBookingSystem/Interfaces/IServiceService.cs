using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Interfaces
{
    public interface IServiceService
    {
        Task<IEnumerable<ServiceDTO>> GetAllServicesAsync();
        Task<ServiceDTO> GetServiceByIdAsync(int serviceId);
        Task<ServiceDTO> CreateServiceAsync(CreateServiceDTO serviceDTO);
        Task<bool> UpdateServiceAsync(int serviceId, UpdateServiceDTO serviceDTO);
        Task<bool> DeleteServiceAsync(int serviceId);
        Task<IEnumerable<ServiceDTO>> GetAllServicesByCategoryIdAsync(int serviceCategoryId);
        Task<float> CalculateAverageRatingAsync(int serviceId);
        Task UpdateServiceRatingAsync(int serviceId);
    }
}
