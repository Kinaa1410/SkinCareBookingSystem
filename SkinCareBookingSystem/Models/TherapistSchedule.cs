using SkinCareBookingSystem.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class TherapistSchedule
{
    [Key]
    public int ScheduleId { get; set; }

    [ForeignKey("TherapistUser")]
    public int TherapistId { get; set; }

    public DayOfWeek DayOfWeek { get; set; }

    public TimeSpan StartTime { get; set; } 
    public TimeSpan EndTime { get; set; }  

    public bool IsAvailable { get; set; } = true;

    public User TherapistUser { get; set; } = null!;
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
