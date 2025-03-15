using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkinCareBookingSystem.Models
{
    public class TherapistTimeSlot
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("TherapistSchedule")]
        public int ScheduleId { get; set; }

        [ForeignKey("TimeSlot")]
        public int TimeSlotId { get; set; }

        public bool IsAvailable { get; set; } = true;

        public TherapistSchedule TherapistSchedule { get; set; } = null!;
        public TimeSlot TimeSlot { get; set; } = null!;
    }
}
