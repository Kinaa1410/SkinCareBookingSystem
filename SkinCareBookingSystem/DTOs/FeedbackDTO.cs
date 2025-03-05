namespace SkinCareBookingSystem.DTOs
{
    public class FeedbackDTO
    {
        public int FeedbackId { get; set; }
        public int BookingId { get; set; }
        public DateTime DateCreated { get; set; }
        public int RatingService { get; set; }
        public int RatingTherapist { get; set; }
        public string CommentService { get; set; } = string.Empty;
        public string CommentTherapist { get; set; } = string.Empty;
        public bool Status { get; set; }
    }

    public class CreateFeedbackDTO
    {
        public int BookingId { get; set; }
        public int RatingService { get; set; }
        public int RatingTherapist { get; set; }
        public string CommentService { get; set; } = string.Empty;
        public string CommentTherapist { get; set; } = string.Empty;
    }

    public class UpdateFeedbackDTO
    {
        public int RatingService { get; set; }
        public int RatingTherapist { get; set; }
        public string CommentService { get; set; } = string.Empty;
        public string CommentTherapist { get; set; } = string.Empty;
        public bool Status { get; set; }
    }
}
