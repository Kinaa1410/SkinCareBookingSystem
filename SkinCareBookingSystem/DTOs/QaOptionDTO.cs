namespace SkinCareBookingSystem.DTOs
{
    public class QaOptionDTO
    {
        public int QaOptionId { get; set; }
        public string AnswerText { get; set; } = string.Empty;
        public List<int> ServiceIds { get; set; } = new List<int>();
    }

    public class CreateQaOptionDTO
    {
        public string AnswerText { get; set; } = string.Empty;
        public List<int> ServiceIds { get; set; } = new List<int>();
    }

    public class UpdateQaOptionDTO
    {
        public int? QaOptionId { get; set; }
        public string AnswerText { get; set; } = string.Empty;
        public List<int> ServiceIds { get; set; } = new List<int>();
    }
}