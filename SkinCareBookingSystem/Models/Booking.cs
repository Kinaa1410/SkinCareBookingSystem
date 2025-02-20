using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SkinCareBookingSystem.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }  // Customer

        [ForeignKey("StaffUser")]
        public int? StaffId { get; set; }  // Staff 

        [ForeignKey("TherapistUser")]
        public int? TherapistId { get; set; }  // Therapist

        public DateTime DateCreated { get; set; }
        public float TotalPrice { get; set; }
        public string Note { get; set; } = string.Empty;
        public bool Status { get; set; }
        public bool IsPaid { get; set; }
        public DateTime AppointmentDate { get; set; }
        public bool UseWallet { get; set; }

        public User? User { get; set; }  // The customer making the booking

        [ForeignKey("StaffId")]
        public User? StaffUser { get; set; }

        [ForeignKey("TherapistId")]
        public User? TherapistUser { get; set; }

        public ICollection<BookingDetails> BookingDetails { get; set; } = new List<BookingDetails>();
    }
}
