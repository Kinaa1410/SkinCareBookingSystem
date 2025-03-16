namespace SkinCareBookingSystem.DTOs
{
    public class TherapistSpecialtyDTO
    {
        public int Id { get; set; }
        public int TherapistId { get; set; }
        public string TherapistName { get; set; }
        public int ServiceCategoryId { get; set; }
        public string ServiceCategoryName { get; set; }
    }

    public class UpdateTherapistSpecialtyDTO
    {
        public int ServiceCategoryId { get; set; }
    }

}
