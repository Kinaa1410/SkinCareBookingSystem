using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.DTOs
{
    public class QaDTO
    {
        public int QaId { get; set; }
        public int ServiceCategoryId { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool Status { get; set; }
        public List<QaOptionDTO> Options { get; set; } = new List<QaOptionDTO>();
    }


    public class CreateQaDTO
    {
        public int ServiceCategoryId { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool Status { get; set; }
        public List<CreateQaOptionDTO> Options { get; set; } = new List<CreateQaOptionDTO>();
    }

    public class UpdateQaDTO
    {
        public string Question { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool Status { get; set; }
        public List<UpdateQaOptionDTO> Options { get; set; } = new List<UpdateQaOptionDTO>();
    }
}
