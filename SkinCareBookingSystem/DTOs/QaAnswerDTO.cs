namespace SkinCareBookingSystem.DTOs
{
    public class QaAnswerDTO
    {
        public int UserId { get; set; }
        public int QaId { get; set; }
        public int QaOptionId { get; set; }
    }

    public class CreateQaAnswerDTO
    {
        public int UserId { get; set; }
        public int QaId { get; set; }
        public int QaOptionId { get; set; }
    }

    public class UpdateQaAnswerDTO
    {
        public int QaOptionId { get; set; }
    }
}
