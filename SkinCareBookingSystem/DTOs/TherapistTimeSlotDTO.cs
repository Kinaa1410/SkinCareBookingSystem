using SkinCareBookingSystem.Enums;

namespace SkinCareBookingSystem.DTOs
{
    public class TherapistTimeSlotDTO
    {
        public int Id { get; set; }
        public int ScheduleId { get; set; }
        public int TimeSlotId { get; set; }
        public string TimeSlotDescription { get; set; } = string.Empty;
        public SlotStatus Status { get; set; } 
    }

    public class CreateTherapistTimeSlotDTO
    {
        public int ScheduleId { get; set; }
        public int TimeSlotId { get; set; }
        public SlotStatus Status { get; set; } = SlotStatus.Available; 
    }

    public class UpdateTherapistTimeSlotDTO
    {
        public int TimeSlotId { get; set; }
        public int ScheduleId { get; set; }
        public SlotStatus Status { get; set; } 
    }
}
