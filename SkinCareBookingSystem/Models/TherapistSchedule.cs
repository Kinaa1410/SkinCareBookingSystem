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

        // This defines the start and end of the working hours for the therapist
        public TimeSpan StartWorkingTime { get; set; }
        public TimeSpan EndWorkingTime { get; set; }

        public User TherapistUser { get; set; } = null!;

        // This is the list of time slots associated with this schedule (many-to-one relationship)
        public ICollection<TherapistTimeSlot> TimeSlots { get; set; } = new List<TherapistTimeSlot>();
    }
}
