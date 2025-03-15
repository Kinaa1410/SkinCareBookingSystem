namespace SkinCareBookingSystem.DTOs
{
    public class TherapistTimeSlotDTO
    {
        public int Id { get; set; }
        public int ScheduleId { get; set; }
        public int TimeSlotId { get; set; }
        public string TimeSlotDescription { get; set; } = string.Empty;
        public bool IsBooked { get; set; }
    }

    public class CreateTherapistTimeSlotDTO
    {
        public int ScheduleId { get; set; }
        public int TimeSlotId { get; set; }
        public bool IsAvailable { get; set; } = true;
    }

    public class UpdateTherapistTimeSlotDTO
    {
        public int TimeSlotId { get; set; }
        public int ScheduleId { get; set; }
        public bool IsAvailable { get; set; }
    }
}
