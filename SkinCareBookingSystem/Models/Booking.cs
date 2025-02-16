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
        public int? StaffId { get; set; }  // Staff (User with RoleId = 2)

        [ForeignKey("TherapistUser")]
        public int? TherapistId { get; set; }  // Therapist (User with RoleId = 3)

        public DateTime DateCreated { get; set; }
        public float TotalPrice { get; set; }
        public string Note { get; set; } = string.Empty;
        public bool Status { get; set; }
        public bool IsPaid { get; set; }
        public DateTime AppointmentDate { get; set; }
        public bool UseWallet { get; set; }

        // ✅ Navigation Properties
        public User? User { get; set; }  // The customer making the booking

        [ForeignKey("StaffId")]
        public User? StaffUser { get; set; }  // ✅ Staff is also a User

        [ForeignKey("TherapistId")]
        public User? TherapistUser { get; set; }  // ✅ Therapist is also a User

        public ICollection<BookingDetails> BookingDetails { get; set; } = new List<BookingDetails>();
    }
}
