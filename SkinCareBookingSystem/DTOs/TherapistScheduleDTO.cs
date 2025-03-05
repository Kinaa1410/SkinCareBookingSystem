namespace SkinCareBookingSystem.DTOs
{
    public class TherapistScheduleDTO
    {
        public int ScheduleId { get; set; }
        public int TherapistId { get; set; }
        public string TherapistName { get; set; } = string.Empty;
        public DayOfWeek DayOfWeek { get; set; }
        public List<TherapistTimeSlotDTO> TimeSlots { get; set; } = new();
    }

    public class CreateTherapistScheduleDTO
    {
        public int TherapistId { get; set; } 
        public DayOfWeek DayOfWeek { get; set; } 
        public string StartTime { get; set; }    
        public string EndTime { get; set; }    
        
    }


    public class UpdateTherapistScheduleDTO
    {
        public DayOfWeek DayOfWeek { get; set; }
        public List<UpdateTherapistTimeSlotDTO> TimeSlots { get; set; } = new(); 
    }
}
