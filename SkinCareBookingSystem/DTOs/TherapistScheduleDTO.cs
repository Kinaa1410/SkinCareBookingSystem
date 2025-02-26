namespace SkinCareBookingSystem.DTOs
{
    public class TherapistScheduleDTO
    {
        public int ScheduleId { get; set; }
        public int TherapistId { get; set; }
        public string TherapistName { get; set; } = string.Empty;
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; }
    }


    public class CreateTherapistScheduleDTO
    {
        public int TherapistId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public int StartHour { get; set; }
        public int StartMinute { get; set; }
        public int EndHour { get; set; }
        public int EndMinute { get; set; }
        public bool IsAvailable { get; set; }
    }


    public class UpdateTherapistScheduleDTO
    {
        public DayOfWeek DayOfWeek { get; set; }
        public int StartHour { get; set; }
        public int StartMinute { get; set; }
        public int EndHour { get; set; }
        public int EndMinute { get; set; }
        public bool IsAvailable { get; set; }
    }


}
