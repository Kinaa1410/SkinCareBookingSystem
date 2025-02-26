namespace SkinCareBookingSystem.DTOs
{
    public class ServiceDTO
    {
        public int ServiceId { get; set; }
        public int ServiceCategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public TimeSpan Duration { get; set; }
        public string VideoURL { get; set; } = string.Empty;
        public float Rating { get; set; }
        public bool Status { get; set; }
        public bool Exist { get; set; }
    }

    public class CreateServiceDTO
    {
        public int ServiceCategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public TimeSpan Duration { get; set; }
        public string VideoURL { get; set; } = string.Empty;
        public float Rating { get; set; }
        public bool Status { get; set; }
        public bool Exist { get; set; }
    }

    public class UpdateServiceDTO
    {
        public int ServiceCategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public TimeSpan Duration { get; set; }
        public string VideoURL { get; set; } = string.Empty;
        public float Rating { get; set; }
        public bool Status { get; set; }
        public bool Exist { get; set; }
    }
}
