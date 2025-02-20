using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;

namespace SkinCareBookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceService _serviceService;
        private readonly IValidator<CreateServiceDTO> _createValidator;
        private readonly IValidator<UpdateServiceDTO> _updateValidator;

        public ServiceController(IServiceService serviceService,
            IValidator<CreateServiceDTO> createValidator,
            IValidator<UpdateServiceDTO> updateValidator)
        {
            _serviceService = serviceService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllServices()
        {
            var services = await _serviceService.GetAllServicesAsync();
            return Ok(services);
        }

        [HttpGet("{serviceId}")]
        public async Task<IActionResult> GetService(int serviceId)
        {
            var service = await _serviceService.GetServiceByIdAsync(serviceId);
            if (service == null)
            {
                return NotFound("Service not found");
            }
            return Ok(service);
        }

        [HttpPost]
        public async Task<IActionResult> CreateService([FromBody] CreateServiceDTO serviceDTO)
        {
            var validationResult = await _createValidator.ValidateAsync(serviceDTO);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var service = await _serviceService.CreateServiceAsync(serviceDTO);
            return CreatedAtAction(nameof(GetService), new { serviceId = service.ServiceId }, service);
        }

        [HttpPut("{serviceId}")]
        public async Task<IActionResult> UpdateService(int serviceId, [FromBody] UpdateServiceDTO serviceDTO)
        {
            var validationResult = await _updateValidator.ValidateAsync(serviceDTO);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var result = await _serviceService.UpdateServiceAsync(serviceId, serviceDTO);
            if (!result)
            {
                return NotFound("Service not found");
            }

            return NoContent();
        }

        [HttpDelete("{serviceId}")]
        public async Task<IActionResult> DeleteService(int serviceId)
        {
            var result = await _serviceService.DeleteServiceAsync(serviceId);
            if (!result)
            {
                return NotFound("Service not found");
            }

            return NoContent();
        }

        [HttpGet("category/{serviceCategoryId}")]
        public async Task<IActionResult> GetAllServicesByCategoryId(int serviceCategoryId)
        {
            var services = await _serviceService.GetAllServicesByCategoryIdAsync(serviceCategoryId);
            if (services == null || !services.Any())
            {
                return NotFound("No services found for the specified category.");
            }
            return Ok(services);
        }
    }
}
