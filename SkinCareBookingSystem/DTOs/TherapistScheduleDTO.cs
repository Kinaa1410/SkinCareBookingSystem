namespace SkinCareBookingSystem.DTOs
{
    public class TherapistScheduleDTO
    {
        public int ScheduleId { get; set; }
        public int TherapistId { get; set; }
        public string TherapistName { get; set; } = string.Empty;
        public DayOfWeek DayOfWeek { get; set; }
        public List<TherapistTimeSlotDTO> TimeSlots { get; set; } = new(); // ✅ Updated
    }

    public class CreateTherapistScheduleDTO
    {
        public int TherapistId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public List<CreateTherapistTimeSlotDTO> TimeSlots { get; set; } = new(); // ✅ Updated
    }

    public class UpdateTherapistScheduleDTO
    {
        public DayOfWeek DayOfWeek { get; set; }
        public List<UpdateTherapistTimeSlotDTO> TimeSlots { get; set; } = new(); // ✅ Updated
    }
}
