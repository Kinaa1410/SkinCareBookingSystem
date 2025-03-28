using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Interfaces
{
    public interface IFeedbackService
    {
        Task<List<FeedbackDTO>> GetFeedbackByServiceIdAsync(int serviceId);
        Task<IEnumerable<FeedbackDTO>> GetAllFeedbacksAsync();
        Task<FeedbackDTO> CreateFeedbackAsync(CreateFeedbackDTO feedbackDTO);
        Task<bool> UpdateFeedbackAsync(int feedbackId, UpdateFeedbackDTO feedbackDTO);
        Task<bool> DeleteFeedbackAsync(int feedbackId);
    }
}