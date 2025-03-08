namespace SkinCareBookingSystem.DTOs
{
    public class ServiceRecommendationDTO
    {
        public int Id { get; set; }
        public int QaId { get; set; }
        public string AnswerOption { get; set; } = string.Empty;
        public int ServiceId { get; set; }
        public int Weight { get; set; }
    }

    public class CreateServiceRecommendationDTO
    {
        public int QaId { get; set; }
        public string AnswerOption { get; set; } = string.Empty;
        public int ServiceId { get; set; }
        public int Weight { get; set; }
    }

    public class UpdateServiceRecommendationDTO
    {
        public int Weight { get; set; }
    }
}
