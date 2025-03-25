using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.Data;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;
using SkinCareBookingSystem.Models;

namespace SkinCareBookingSystem.Implements
{
    public class FeedbackService : IFeedbackService
    {
        private readonly BookingDbContext _context;
        private readonly IServiceService _service;

        public FeedbackService(BookingDbContext context, IServiceService service)
        {
            _context = context;
            _service = service;
        }

        public async Task<FeedbackDTO> GetFeedbackByServiceIdAsync(int serviceId)
        {
            var feedback = await _context.Feedbacks
                .Include(f => f.Service)
                .FirstOrDefaultAsync(f => f.ServiceId == serviceId);

            if (feedback == null) return null;

            return new FeedbackDTO
            {
                FeedbackId = feedback.FeedbackId,
                ServiceId = feedback.ServiceId,
                Rating = feedback.Rating,
                Comment = feedback.Comment
            };
        }

        public async Task<IEnumerable<FeedbackDTO>> GetAllFeedbacksAsync()
        {
            return await _context.Feedbacks
                .Include(f => f.Service)
                .Select(f => new FeedbackDTO
                {
                    FeedbackId = f.FeedbackId,
                    ServiceId = f.ServiceId,
                    Rating = f.Rating,
                    Comment = f.Comment
                }).ToListAsync();
        }

        public async Task<FeedbackDTO> CreateFeedbackAsync(CreateFeedbackDTO feedbackDTO)
        {
            var service = await _context.Services.FindAsync(feedbackDTO.ServiceId);
            if (service == null) throw new InvalidOperationException("Service not found.");

            var feedback = new Feedback
            {
                ServiceId = feedbackDTO.ServiceId,
                Rating = feedbackDTO.Rating,
                Comment = feedbackDTO.Comment
            };
            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();
            //await _service.UpdateServiceRatingAsync(feedbackDTO.ServiceId);

            return new FeedbackDTO
            {
                FeedbackId = feedback.FeedbackId,
                ServiceId = feedback.ServiceId,
                Rating = feedback.Rating,
                Comment = feedback.Comment
            };
        }

        public async Task<bool> UpdateFeedbackAsync(int feedbackId, UpdateFeedbackDTO feedbackDTO)
        {
            var feedback = await _context.Feedbacks.FindAsync(feedbackId);
            if (feedback == null) return false;

            feedback.Rating = feedbackDTO.Rating;
            feedback.Comment = feedbackDTO.Comment;

            _context.Entry(feedback).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            //await _service.UpdateServiceRatingAsync(feedback.ServiceId);
            return true;
        }

        public async Task<bool> DeleteFeedbackAsync(int feedbackId)
        {
            var feedback = await _context.Feedbacks.FindAsync(feedbackId);
            if (feedback == null) return false;

            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}