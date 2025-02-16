using System.ComponentModel.DataAnnotations;

namespace SkinCareBookingSystem.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        public string RoleName { get; set; } = string.Empty; // ✅ Fixes CS8618 warning

        public bool Status { get; set; }
    }
}
