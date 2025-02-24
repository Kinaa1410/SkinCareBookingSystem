using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.Data;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;
using SkinCareBookingSystem.Models;

namespace SkinCareBookingSystem.Implements
{
    public class CartItemService : ICartItemService
    {
        private readonly BookingDbContext _context;

        public CartItemService(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CartItemDTO>> GetAllCartItemsAsync()
        {
            return await _context.CartItems
                .Select(cartItem => new CartItemDTO
                {
                    Id = cartItem.Id,
                    UserId = cartItem.UserId,
                    ServiceId = cartItem.ServiceId
                }).ToListAsync();
        }

        public async Task<CartItemDTO> GetCartItemByIdAsync(int id)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.Id == id);
            if (cartItem == null) return null;

            return new CartItemDTO
            {
                Id = cartItem.Id,
                UserId = cartItem.UserId,
                ServiceId = cartItem.ServiceId
            };
        }

        public async Task<CartItemDTO> CreateCartItemAsync(CreateCartItemDTO cartItemDTO)
        {
            var cartItem = new CartItem
            {
                UserId = cartItemDTO.UserId,
                ServiceId = cartItemDTO.ServiceId
            };

            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();

            return new CartItemDTO
            {
                Id = cartItem.Id,
                UserId = cartItem.UserId,
                ServiceId = cartItem.ServiceId
            };
        }

        public async Task<bool> UpdateCartItemAsync(int id, UpdateCartItemDTO cartItemDTO)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem == null) return false;

            cartItem.ServiceId = cartItemDTO.ServiceId;

            _context.Entry(cartItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCartItemAsync(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem == null) return false;

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
