using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SkinCareBookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TherapistTimeSlotController : ControllerBase
    {
        private readonly ITherapistTimeSlotService _timeSlotService;

        public TherapistTimeSlotController(ITherapistTimeSlotService timeSlotService)
        {
            _timeSlotService = timeSlotService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TherapistTimeSlotDTO>>> GetAllTimeSlots()
        {
            var timeSlots = await _timeSlotService.GetAllTimeSlotsAsync();
            return Ok(timeSlots);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TherapistTimeSlotDTO>> GetTimeSlotById(int id)
        {
            var timeSlot = await _timeSlotService.GetTimeSlotByIdAsync(id);
            if (timeSlot == null) return NotFound();
            return Ok(timeSlot);
        }

        [HttpPost("{scheduleId}")]
        public async Task<ActionResult<TherapistTimeSlotDTO>> CreateTimeSlot(int scheduleId, [FromBody] CreateTherapistTimeSlotDTO timeSlotDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var newTimeSlot = await _timeSlotService.CreateTimeSlotAsync(scheduleId, timeSlotDTO);
            return CreatedAtAction(nameof(GetTimeSlotById), new { id = newTimeSlot.TimeSlotId }, newTimeSlot);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTimeSlot(int id, [FromBody] UpdateTherapistTimeSlotDTO timeSlotDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _timeSlotService.UpdateTimeSlotAsync(id, timeSlotDTO);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTimeSlot(int id)
        {
            var deleted = await _timeSlotService.DeleteTimeSlotAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
