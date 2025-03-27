namespace SkinCareBookingSystem.Enums
{
    public enum BookingStatus
    {
        Pending = 0,    // Awaiting payment or confirmation
        Booked = 1,     // Paid and confirmed
        Completed = 2,  // Appointment finished
        Canceled = 3,   // User or system canceled
        Failed = 4      // Payment failed or timed out
    }
}