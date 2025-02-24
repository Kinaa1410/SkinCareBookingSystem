namespace SkinCareBookingSystem.DTOs
{
    public class QaAnswerDTO
    {
        public int UserId { get; set; }
        public int QaId { get; set; }
        public string Answer { get; set; } = string.Empty;
    }

    public class CreateQaAnswerDTO
    {
        public int UserId { get; set; }
        public int QaId { get; set; }
        public string Answer { get; set; } = string.Empty;
    }

    public class UpdateQaAnswerDTO
    {
        public string Answer { get; set; } = string.Empty;
    }
}
