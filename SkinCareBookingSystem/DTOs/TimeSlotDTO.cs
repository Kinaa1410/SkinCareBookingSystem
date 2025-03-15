namespace SkinCareBookingSystem.DTOs
{
    public class TimeSlotDTO
    {
        public int TimeSlotId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Description => $"{StartTime.Hours}:{StartTime.Minutes:D2} - {EndTime.Hours}:{EndTime.Minutes:D2}";
    }

    public class CreateTimeSlotDTO
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }

    public class UpdateTimeSlotDTO
    {
        public int TimeSlotId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
