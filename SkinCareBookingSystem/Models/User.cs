using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkinCareBookingSystem.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [ForeignKey("Role")]
        public int RoleId { get; set; }  // 1 = User, 2 = Staff, 3 = Therapist

        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        public bool Status { get; set; }

        // ✅ Foreign key references
        public Role Role { get; set; } = null!;  // Required, always has a Role
        public UserDetails? UserDetails { get; set; }  // Nullable, might not have details
        public Wallet? Wallet { get; set; }  // Nullable, might not have a wallet

        // ✅ Bookings where this user is the CUSTOMER
        public ICollection<Booking> CustomerBookings { get; set; } = new List<Booking>();
    }
}
