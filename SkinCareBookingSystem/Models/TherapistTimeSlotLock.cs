using SkinCareBookingSystem.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkinCareBookingSystem.Models
{
    public class TherapistTimeSlotLock
    {
        [Key]
        public int Id { get; set; }

        public int TherapistTimeSlotId { get; set; }

        public DateTime Date { get; set; }

        public SlotStatus Status { get; set; }

        public TherapistTimeSlot TherapistTimeSlot { get; set; } = null!;
    }
}