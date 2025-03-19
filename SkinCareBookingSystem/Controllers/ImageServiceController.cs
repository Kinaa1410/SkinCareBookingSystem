using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;
using FluentValidation;

namespace SkinCareBookingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageServiceController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly IValidator<CreateImageServiceDTO> _createValidator;
        private readonly IValidator<UpdateImageServiceDTO> _updateValidator;

        public ImageServiceController(IImageService imageService,
            IValidator<CreateImageServiceDTO> createValidator,
            IValidator<UpdateImageServiceDTO> updateValidator)
        {
            _imageService = imageService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateImageService([FromBody] CreateImageServiceDTO createDTO)
        {
            var validationResult = await _createValidator.ValidateAsync(createDTO);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var imageService = await _imageService.CreateImageServiceAsync(createDTO);
            return CreatedAtAction(nameof(GetImageService), new { imageServiceId = imageService.ImageServiceId }, imageService);
        }

        [HttpPut("{imageServiceId}")]
        public async Task<IActionResult> UpdateImageService(int imageServiceId, [FromBody] UpdateImageServiceDTO updateDTO)
        {
            var validationResult = await _updateValidator.ValidateAsync(updateDTO);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            try
            {
                var updatedImageService = await _imageService.UpdateImageServiceAsync(imageServiceId, updateDTO);
                return Ok(updatedImageService);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Image service not found.");
            }
        }

        [HttpDelete("{imageServiceId}")]
        public async Task<IActionResult> DeleteImageService(int imageServiceId)
        {
            var success = await _imageService.DeleteImageServiceAsync(imageServiceId);
            if (!success)
            {
                return NotFound("Image service not found.");
            }

            return NoContent();
        }

        [HttpGet("{imageServiceId}")]
        public async Task<IActionResult> GetImageService(int imageServiceId)
        {
            try
            {
                var imageService = await _imageService.GetImageServiceAsync(imageServiceId);
                return Ok(imageService);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Image service not found.");
            }
        }

        [HttpGet("service/{serviceId}")]
        public async Task<IActionResult> GetImageServiceByServiceId(int serviceId)
        {
            try
            {
                var imageServices = await _imageService.GetImageServiceByServiceIdAsync(serviceId);
                return Ok(imageServices);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("No image services found for this service.");
            }
        }
    }
}