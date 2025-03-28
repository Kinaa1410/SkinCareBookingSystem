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

        public async Task<List<FeedbackDTO>> GetFeedbackByServiceIdAsync(int serviceId)
        {
            var feedbacks = await _context.Feedbacks
                .Include(f => f.Service)
                .Include(f => f.User)
                .Where(f => f.ServiceId == serviceId)
                .ToListAsync();

            return feedbacks.Any() ? feedbacks.Select(f => new FeedbackDTO
            {
                FeedbackId = f.FeedbackId,
                ServiceId = f.ServiceId,
                UserId = f.UserId,
                Rating = f.Rating,
                Comment = f.Comment
            }).ToList() : new List<FeedbackDTO>();
        }

        public async Task<IEnumerable<FeedbackDTO>> GetAllFeedbacksAsync()
        {
            return await _context.Feedbacks
                .Include(f => f.Service)
                .Include(f => f.User)
                .Select(f => new FeedbackDTO
                {
                    FeedbackId = f.FeedbackId,
                    ServiceId = f.ServiceId,
                    UserId = f.UserId,
                    Rating = f.Rating,
                    Comment = f.Comment
                }).ToListAsync();
        }

        public async Task<FeedbackDTO> CreateFeedbackAsync(CreateFeedbackDTO feedbackDTO)
        {
            var service = await _context.Services.FindAsync(feedbackDTO.ServiceId);
            if (service == null) throw new InvalidOperationException("Service not found.");

            var user = await _context.Users.FindAsync(feedbackDTO.UserId);
            if (user == null) throw new InvalidOperationException("User not found.");

            var feedback = new Feedback
            {
                ServiceId = feedbackDTO.ServiceId,
                UserId = feedbackDTO.UserId,
                Rating = feedbackDTO.Rating,
                Comment = feedbackDTO.Comment
            };
            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            return new FeedbackDTO
            {
                FeedbackId = feedback.FeedbackId,
                ServiceId = feedback.ServiceId,
                UserId = feedback.UserId,
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