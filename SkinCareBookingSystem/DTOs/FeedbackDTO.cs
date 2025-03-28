namespace SkinCareBookingSystem.DTOs
{
    public class FeedbackDTO
    {
        public int FeedbackId { get; set; }
        public int ServiceId { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }

    public class CreateFeedbackDTO
    {
        public int ServiceId { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }

    public class UpdateFeedbackDTO
    {
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}