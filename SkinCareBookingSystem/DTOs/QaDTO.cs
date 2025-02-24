namespace SkinCareBookingSystem.DTOs
{
    public class QaDTO
    {
        public int QaId { get; set; }
        public int ServiceCategoryId { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool Status { get; set; }
    }

    public class CreateQaDTO
    {
        public int ServiceCategoryId { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool Status { get; set; }
    }

    public class UpdateQaDTO
    {
        public string Question { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool Status { get; set; }
    }
}
