using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Controllers
{
    [Authorize(Roles = "Admin, Staff")]
    [Route("api/[controller]")]
    [ApiController]
    public class TherapistSpecialtyController : ControllerBase
    {
        private readonly ITherapistSpecialtyService _therapistSpecialtyService;

        public TherapistSpecialtyController(ITherapistSpecialtyService therapistSpecialtyService)
        {
            _therapistSpecialtyService = therapistSpecialtyService;
        }

        // Get all therapist specialties
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<TherapistSpecialtyDTO>>> GetAllTherapistSpecialties()
        {
            var specialties = await _therapistSpecialtyService.GetAllTherapistSpecialtiesAsync();
            return Ok(specialties);
        }

        // Get therapist specialties by therapistId
        [HttpGet("{therapistId}")]
        public async Task<ActionResult<IEnumerable<TherapistSpecialtyDTO>>> GetTherapistSpecialtiesByTherapistId(int therapistId)
        {
            var specialties = await _therapistSpecialtyService.GetTherapistSpecialtiesByTherapistIdAsync(therapistId);
            if (specialties == null || !specialties.Any())
                return NotFound(new { message = "No specialties found for this therapist." });

            return Ok(specialties);
        }

        // Get therapists by serviceCategoryId
        [HttpGet("by-service-category/{serviceCategoryId}")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetTherapistsByServiceCategoryId(int serviceCategoryId)
        {
            var therapists = await _therapistSpecialtyService.GetTherapistsByServiceCategoryIdAsync(serviceCategoryId);
            if (therapists == null || !therapists.Any())
                return NotFound(new { message = "No therapists found for this service category." });

            return Ok(therapists);
        }

        // Create therapist specialty
        [HttpPost("create")]
        public async Task<ActionResult<TherapistSpecialtyDTO>> CreateTherapistSpecialty([FromBody] TherapistSpecialtyDTO specialtyDTO)
        {
            // Validate the service category and therapist ID
            var newSpecialty = await _therapistSpecialtyService.CreateTherapistSpecialtyAsync(specialtyDTO);
            return CreatedAtAction(nameof(GetTherapistSpecialtiesByTherapistId), new { therapistId = newSpecialty.TherapistId }, newSpecialty);
        }

        // Delete therapist specialty
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteTherapistSpecialty(int id)
        {
            var result = await _therapistSpecialtyService.DeleteTherapistSpecialtyAsync(id);
            if (!result)
                return NotFound(new { message = "Therapist specialty not found." });

            return NoContent();
        }
    }
}
