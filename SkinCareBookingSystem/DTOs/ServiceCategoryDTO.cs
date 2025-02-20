namespace SkinCareBookingSystem.DTOs
{
    public class ServiceCategoryDTO
    {
        public int ServiceCategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Status { get; set; }
        public bool Exist { get; set; }
    }

    public class CreateServiceCategoryDTO
    {
        public string Name { get; set; } = string.Empty;
        public bool Status { get; set; }
        public bool Exist { get; set; }
    }

    public class UpdateServiceCategoryDTO
    {
        public string Name { get; set; } = string.Empty;
        public bool Status { get; set; }
        public bool Exist { get; set; }
    }
}
