using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Interfaces
{
    public interface IServiceRecommendationService
    {
        Task<IEnumerable<ServiceRecommendationDTO>> GetAllServiceRecommendationsAsync();
        Task<ServiceRecommendationDTO> GetServiceRecommendationByIdAsync(int id);
        Task<ServiceRecommendationDTO> CreateServiceRecommendationAsync(CreateServiceRecommendationDTO dto);
        Task<bool> UpdateServiceRecommendationAsync(int id, UpdateServiceRecommendationDTO dto);
        Task<bool> DeleteServiceRecommendationAsync(int id);
        Task<IEnumerable<ServiceDTO>> GetRecommendedServicesAsync(List<QaAnswerDTO> answers);
    }
}
