using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;

namespace SkinCareBookingSystem.Controllers
{
    [Route("api/service-recommendations")]
    [ApiController]
    public class ServiceRecommendationController : ControllerBase
    {
        private readonly IServiceRecommendationService _service;

        public ServiceRecommendationController(IServiceRecommendationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllServiceRecommendationsAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetServiceRecommendationByIdAsync(id);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateServiceRecommendationDTO dto)
        {
            var result = await _service.CreateServiceRecommendationAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPost("get-service-recommendation")]
        public async Task<IActionResult> GetRecommendedServices([FromBody] List<QaAnswerDTO> answers)
        {
            if (answers == null || !answers.Any())
            {
                return BadRequest("No answers provided.");
            }

            var recommendedServices = await _service.GetRecommendedServicesAsync(answers);

            if (!recommendedServices.Any())
            {
                return NotFound("No suitable services found.");
            }

            return Ok(recommendedServices);
        }


    }
}
