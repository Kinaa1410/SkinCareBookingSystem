namespace SkinCareBookingSystem.DTOs
{
    public class CartItemDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ServiceId { get; set; }
    }

    public class CreateCartItemDTO
    {
        public int UserId { get; set; }
        public int ServiceId { get; set; }
    }

    public class UpdateCartItemDTO
    {
        public int ServiceId { get; set; }
    }
}
