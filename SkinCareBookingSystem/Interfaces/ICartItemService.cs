using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Interfaces
{
    public interface ICartItemService
    {
        Task<IEnumerable<CartItemDTO>> GetAllCartItemsAsync();
        Task<CartItemDTO> GetCartItemByIdAsync(int id);
        Task<CartItemDTO> CreateCartItemAsync(CreateCartItemDTO cartItemDTO);
        Task<bool> UpdateCartItemAsync(int id, UpdateCartItemDTO cartItemDTO);
        Task<bool> DeleteCartItemAsync(int id);
    }
}
