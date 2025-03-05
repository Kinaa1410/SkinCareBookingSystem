using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Interfaces
{
    public interface IFeedbackService
    {
        Task<FeedbackDTO> GetFeedbackByBookingIdAsync(int bookingId);
        Task<IEnumerable<FeedbackDTO>> GetAllFeedbacksAsync();
        Task<FeedbackDTO> CreateFeedbackAsync(CreateFeedbackDTO feedbackDTO);
        Task<bool> UpdateFeedbackAsync(int feedbackId, UpdateFeedbackDTO feedbackDTO);
        Task<bool> DeleteFeedbackAsync(int feedbackId);
        Task<IEnumerable<int>> GetTherapistRatingsAsync(int therapistId);
        Task<double> GetAverageTherapistRatingAsync(int therapistId);
    }
}
