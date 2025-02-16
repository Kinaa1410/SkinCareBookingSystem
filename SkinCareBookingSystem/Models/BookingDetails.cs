using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SkinCareBookingSystem.Models
{
    public class BookingDetails
    {
        [Key, Column(Order = 1)]
        public int BookingId { get; set; }

        [Key, Column(Order = 2)]
        public int ServiceId { get; set; }

        public int StockBooking { get; set; }
        public float Price { get; set; }

        public Booking? Booking { get; set; }
        public Service? Service { get; set; }
    }
}
