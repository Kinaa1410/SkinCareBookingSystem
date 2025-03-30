using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkinCareBookingSystem.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [ForeignKey("Role")]
        public int RoleId { get; set; }

        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        public bool Status { get; set; }

        public Role Role { get; set; } = null!;  
        public UserDetails? UserDetails { get; set; } 

        public ICollection<Booking> CustomerBookings { get; set; } = new List<Booking>();
        public ICollection<TherapistSpecialty> TherapistSpecialties { get; set; } = new List<TherapistSpecialty>();
    }
}
