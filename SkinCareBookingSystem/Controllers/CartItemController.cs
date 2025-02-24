using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;

namespace SkinCareBookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemController : ControllerBase
    {
        private readonly ICartItemService _cartItemService;
        private readonly IValidator<CreateCartItemDTO> _createValidator;
        private readonly IValidator<UpdateCartItemDTO> _updateValidator;

        public CartItemController(ICartItemService cartItemService,
            IValidator<CreateCartItemDTO> createValidator,
            IValidator<UpdateCartItemDTO> updateValidator)
        {
            _cartItemService = cartItemService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCartItems()
        {
            var cartItems = await _cartItemService.GetAllCartItemsAsync();
            return Ok(cartItems);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCartItem(int id)
        {
            var cartItem = await _cartItemService.GetCartItemByIdAsync(id);
            if (cartItem == null)
            {
                return NotFound("Cart not found");
            }
            return Ok(cartItem);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCartItem([FromBody] CreateCartItemDTO cartItemDTO)
        {
            var validationResult = await _createValidator.ValidateAsync(cartItemDTO);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var cartItem = await _cartItemService.CreateCartItemAsync(cartItemDTO);
            return CreatedAtAction(nameof(GetCartItem), new { id = cartItem.Id }, cartItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCartItem(int id, [FromBody] UpdateCartItemDTO cartItemDTO)
        {
            var validationResult = await _updateValidator.ValidateAsync(cartItemDTO);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var result = await _cartItemService.UpdateCartItemAsync(id, cartItemDTO);
            if (!result)
            {
                return NotFound("Cart not found");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCartItem(int id)
        {
            var result = await _cartItemService.DeleteCartItemAsync(id);
            if (!result)
            {
                return NotFound("Cart not found");
            }

            return NoContent();
        }
    }
}
