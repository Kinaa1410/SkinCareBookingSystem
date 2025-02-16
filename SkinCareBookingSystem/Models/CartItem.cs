using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkinCareBookingSystem.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [ForeignKey("Service")]
        public int ServiceId { get; set; }

        public User User { get; set; } = null!;
        public Service Service { get; set; } = null!;
    }
}
