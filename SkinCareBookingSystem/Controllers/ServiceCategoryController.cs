using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;

namespace SkinCareBookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceCategoryController : ControllerBase
    {
        private readonly IServiceCategoryService _serviceCategoryService;
        private readonly IValidator<CreateServiceCategoryDTO> _createValidator;
        private readonly IValidator<UpdateServiceCategoryDTO> _updateValidator;

        public ServiceCategoryController(IServiceCategoryService serviceCategoryService,
            IValidator<CreateServiceCategoryDTO> createValidator,
            IValidator<UpdateServiceCategoryDTO> updateValidator)
        {
            _serviceCategoryService = serviceCategoryService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllServiceCategories()
        {
            var serviceCategories = await _serviceCategoryService.GetAllServiceCategoriesAsync();
            return Ok(serviceCategories);
        }

        [HttpGet("{serviceCategoryId}")]
        public async Task<IActionResult> GetServiceCategory(int serviceCategoryId)
        {
            var serviceCategory = await _serviceCategoryService.GetServiceCategoryByIdAsync(serviceCategoryId);
            if (serviceCategory == null)
            {
                return NotFound("Service category not found");
            }
            return Ok(serviceCategory);
        }


        [HttpPost]
        public async Task<IActionResult> CreateServiceCategory([FromBody] CreateServiceCategoryDTO serviceCategoryDTO)
        {
            var validationResult = await _createValidator.ValidateAsync(serviceCategoryDTO);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var serviceCategory = await _serviceCategoryService.CreateServiceCategoryAsync(serviceCategoryDTO);
            return CreatedAtAction(nameof(GetServiceCategory), new { serviceCategoryId = serviceCategory.ServiceCategoryId }, serviceCategory);
        }

        [HttpPut("{serviceCategoryId}")]
        public async Task<IActionResult> UpdateServiceCategory(int serviceCategoryId, [FromBody] UpdateServiceCategoryDTO serviceCategoryDTO)
        {
            var validationResult = await _updateValidator.ValidateAsync(serviceCategoryDTO);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var result = await _serviceCategoryService.UpdateServiceCategoryAsync(serviceCategoryId, serviceCategoryDTO);
            if (!result)
            {
                return NotFound("Service category not found");
            }

            return NoContent();
        }

        [HttpDelete("{serviceCategoryId}")]
        public async Task<IActionResult> DeleteServiceCategory(int serviceCategoryId)
        {
            var result = await _serviceCategoryService.DeleteServiceCategoryAsync(serviceCategoryId);
            if (!result)
            {
                return NotFound("Service category not found");
            }

            return NoContent();
        }
    }
}
