namespace SkinCareBookingSystem.DTOs
{
    public class TherapistScheduleDTO
    {
        public int ScheduleId { get; set; }
        public int TherapistId { get; set; }
        public string TherapistName { get; set; } = string.Empty;
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartWorkingTime { get; set; }
        public TimeSpan EndWorkingTime { get; set; }
        public List<TherapistTimeSlotDTO> TimeSlots { get; set; } = new List<TherapistTimeSlotDTO>();
    }

    public class CreateTherapistScheduleDTO
    {
        public int TherapistId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public string StartTime { get; set; } = string.Empty; // Time format: "HH:mm"
        public string EndTime { get; set; } = string.Empty;   // Time format: "HH:mm"
    }

    public class UpdateTherapistScheduleDTO
    {
        public int ScheduleId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public string StartTime { get; set; }  // Start time as string (or TimeSpan depending on how you want to handle it)
        public string EndTime { get; set; }    // End time as string (or TimeSpan depending on how you want to handle it)
        public List<UpdateTherapistTimeSlotDTO> TimeSlots { get; set; } = new List<UpdateTherapistTimeSlotDTO>();
    }
}
