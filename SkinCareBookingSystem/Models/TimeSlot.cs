using System.ComponentModel.DataAnnotations;

namespace SkinCareBookingSystem.Models
{
    public class TimeSlot
    {
        [Key]
        public int TimeSlotId { get; set; }

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public string Description => $"{StartTime.Hours}:{StartTime.Minutes:D2} - {EndTime.Hours}:{EndTime.Minutes:D2}";
    }
}
