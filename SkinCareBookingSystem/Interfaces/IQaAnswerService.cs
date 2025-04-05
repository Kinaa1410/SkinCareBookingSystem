using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Interfaces
{
    public interface IQaAnswerService
    {
        Task<IEnumerable<QaAnswerDTO>> GetAllQaAnswersAsync();
        Task<QaAnswerDTO> GetQaAnswerByIdsAsync(int userId, int qaId);
        Task<QaAnswerDTO> CreateQaAnswerAsync(CreateQaAnswerDTO qaAnswerDTO);
        Task<bool> UpdateQaAnswerAsync(int userId, int qaId, UpdateQaAnswerDTO qaAnswerDTO);
        Task<bool> DeleteQaAnswerAsync(int userId, int qaId);
        Task<IEnumerable<ServiceDTO>> SubmitAnswersAndGetRecommendationsAsync(List<CreateQaAnswerDTO> qaAnswersDTO);
    }
}