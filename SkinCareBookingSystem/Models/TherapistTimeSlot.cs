using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkinCareBookingSystem.Models
{
    public class TherapistTimeSlot
    {
        [Key]
        public int TimeSlotId { get; set; }

        [ForeignKey("TherapistSchedule")]
        public int ScheduleId { get; set; } 

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; } = true;

        public TherapistSchedule TherapistSchedule { get; set; } = null!; 
    }
}
