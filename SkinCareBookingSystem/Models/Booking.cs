using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkinCareBookingSystem.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [ForeignKey("StaffUser")]
        public int? StaffId { get; set; }

        [ForeignKey("TherapistTimeSlot")]
        public int TimeSlotId { get; set; } 

        public DateTime DateCreated { get; set; }
        public float TotalPrice { get; set; }
        public string Note { get; set; } = string.Empty;
        public bool Status { get; set; }
        public bool IsPaid { get; set; }
        public DateTime AppointmentDate { get; set; }
        public bool UseWallet { get; set; }

        public User User { get; set; }
        public User StaffUser { get; set; }

        public TherapistTimeSlot TherapistTimeSlot { get; set; } = null!;

        public ICollection<BookingDetails> BookingDetails { get; set; } = new List<BookingDetails>();
    }
}
