namespace SkinCareBookingSystem.DTOs
{
    public class TherapistTimeSlotDTO
    {
        public int TimeSlotId { get; set; }
        public int ScheduleId { get; set; } // ✅ Reference to TherapistSchedule
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class CreateTherapistTimeSlotDTO
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class UpdateTherapistTimeSlotDTO
    {
        public int TimeSlotId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; }
    }
}
