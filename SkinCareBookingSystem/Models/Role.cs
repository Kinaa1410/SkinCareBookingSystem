using System.ComponentModel.DataAnnotations;

namespace SkinCareBookingSystem.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        public string RoleName { get; set; } = string.Empty; 

        public bool Status { get; set; }
    }
}
