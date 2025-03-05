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

        public FeedbackService(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<FeedbackDTO> GetFeedbackByBookingIdAsync(int bookingId)
        {
            var feedback = await _context.Feedbacks
                .Include(f => f.Booking)
                .ThenInclude(b => b.TherapistTimeSlot)
                .ThenInclude(ts => ts.TherapistSchedule)
                .ThenInclude(ts => ts.TherapistUser)
                .FirstOrDefaultAsync(f => f.BookingId == bookingId);

            if (feedback == null) return null;

            return new FeedbackDTO
            {
                FeedbackId = feedback.FeedbackId,
                BookingId = feedback.BookingId,
                DateCreated = feedback.DateCreated,
                RatingService = feedback.RatingService,
                RatingTherapist = feedback.RatingTherapist,
                CommentService = feedback.CommentService,
                CommentTherapist = feedback.CommentTherapist,
                Status = feedback.Status
            };
        }

        public async Task<IEnumerable<FeedbackDTO>> GetAllFeedbacksAsync()
        {
            return await _context.Feedbacks
                .Include(f => f.Booking)
                .ThenInclude(b => b.TherapistTimeSlot)
                .ThenInclude(ts => ts.TherapistSchedule)
                .ThenInclude(ts => ts.TherapistUser)
                .Select(f => new FeedbackDTO
                {
                    FeedbackId = f.FeedbackId,
                    BookingId = f.BookingId,
                    DateCreated = f.DateCreated,
                    RatingService = f.RatingService,
                    RatingTherapist = f.RatingTherapist,
                    CommentService = f.CommentService,
                    CommentTherapist = f.CommentTherapist,
                    Status = f.Status
                }).ToListAsync();
        }

        public async Task<FeedbackDTO> CreateFeedbackAsync(CreateFeedbackDTO feedbackDTO)
        {
            var booking = await _context.Bookings.FindAsync(feedbackDTO.BookingId);
            if (booking == null) throw new InvalidOperationException("Booking not found.");

            var feedback = new Feedback
            {
                BookingId = feedbackDTO.BookingId,
                RatingService = feedbackDTO.RatingService,
                RatingTherapist = feedbackDTO.RatingTherapist,
                CommentService = feedbackDTO.CommentService,
                CommentTherapist = feedbackDTO.CommentTherapist,
                DateCreated = DateTime.Now,
                Status = true
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            return await GetFeedbackByBookingIdAsync(feedback.FeedbackId);
        }

        public async Task<bool> UpdateFeedbackAsync(int feedbackId, UpdateFeedbackDTO feedbackDTO)
        {
            var feedback = await _context.Feedbacks.FindAsync(feedbackId);
            if (feedback == null) return false;

            feedback.RatingService = feedbackDTO.RatingService;
            feedback.RatingTherapist = feedbackDTO.RatingTherapist;
            feedback.CommentService = feedbackDTO.CommentService;
            feedback.CommentTherapist = feedbackDTO.CommentTherapist;
            feedback.Status = feedbackDTO.Status;

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

        public async Task<IEnumerable<int>> GetTherapistRatingsAsync(int therapistId)
        {
            var ratings = await _context.Feedbacks
                .Where(f => f.Booking.TherapistTimeSlot.TherapistSchedule.TherapistId == therapistId)
                .Select(f => f.RatingTherapist)
                .ToListAsync();

            return ratings;
        }

        public async Task<double> GetAverageTherapistRatingAsync(int therapistId)
        {
            var averageRating = await _context.Feedbacks
                .Where(f => f.Booking.TherapistTimeSlot.TherapistSchedule.TherapistId == therapistId)
                .AverageAsync(f => f.RatingTherapist);

            return averageRating;
        }
    }
}
