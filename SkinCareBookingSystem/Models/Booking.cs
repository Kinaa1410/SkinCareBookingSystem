﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SkinCareBookingSystem.Enums;

namespace SkinCareBookingSystem.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [ForeignKey("Therapist")]
        public int TherapistId { get; set; }

        [ForeignKey("Service")]
        public int ServiceId { get; set; }

        public int TimeSlotId { get; set; }

        [ForeignKey("TherapistTimeSlot")]
        public int TherapistTimeSlotId { get; set; }

        public DateTime DateCreated { get; set; }
        public float TotalPrice { get; set; }
        public string Note { get; set; } = string.Empty;
        public BookingStatus Status { get; set; }
        public bool IsPaid { get; set; }
        public DateTime AppointmentDate { get; set; }

        public User User { get; set; } = null!;
        public User Therapist { get; set; } = null!;
        public Service Service { get; set; } = null!;
        public TherapistTimeSlot TherapistTimeSlot { get; set; } = null!;
    }
}