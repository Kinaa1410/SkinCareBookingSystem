namespace SkinCareBookingSystem.DTOs
{
    public class BookingDTO
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int TherapistId { get; set; }
        public int TimeSlotId { get; set; }
        public DateTime DateCreated { get; set; }
        public float TotalPrice { get; set; }
        public string Note { get; set; } = string.Empty;
        public bool Status { get; set; }
        public bool IsPaid { get; set; }
        public DateTime AppointmentDate { get; set; }
        public bool UseWallet { get; set; }
    }

    public class CreateBookingDTO
    {
        public int UserId { get; set; }
        public int? TherapistId { get; set; }
        public int TimeSlotId { get; set; } 
        public bool UseWallet { get; set; }
        public string Note { get; set; } = string.Empty;
        public int ServiceId { get; set; }
    }

    public class UpdateBookingDTO
    {
        public bool Status { get; set; }
        public bool IsPaid { get; set; }
    }
}
