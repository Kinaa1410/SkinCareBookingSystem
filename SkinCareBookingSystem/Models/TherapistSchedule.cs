using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkinCareBookingSystem.Models
{
    public class TherapistSchedule
    {
        [Key]
        public int ScheduleId { get; set; }

        [ForeignKey("TherapistUser")]
        public int TherapistId { get; set; }

        public DayOfWeek DayOfWeek { get; set; }

        public ICollection<TherapistTimeSlot> TimeSlots { get; set; } = new List<TherapistTimeSlot>();

        public User TherapistUser { get; set; } = null!;
    }
}
