using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SkinCareBookingSystem.Models
{
    public class Wallet
    {
        [Key, ForeignKey("User")]
        public int UserId { get; set; }
        public float Amount { get; set; }
        public bool Status { get; set; }

        public User? User { get; set; }
    }
}
