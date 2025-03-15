using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SkinCareBookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeSlotController : ControllerBase
    {
        private readonly ITimeSlotService _timeSlotService;

        public TimeSlotController(ITimeSlotService timeSlotService)
        {
            _timeSlotService = timeSlotService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TimeSlotDTO>>> GetAllTimeSlots()
        {
            var timeSlots = await _timeSlotService.GetAllTimeSlotsAsync();
            return Ok(timeSlots);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TimeSlotDTO>> GetTimeSlotById(int id)
        {
            var timeSlot = await _timeSlotService.GetTimeSlotByIdAsync(id);
            if (timeSlot == null) return NotFound();
            return Ok(timeSlot);
        }

        [HttpPost]
        public async Task<ActionResult<TimeSlotDTO>> CreateTimeSlot([FromBody] CreateTimeSlotDTO timeSlotDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var newTimeSlot = await _timeSlotService.CreateTimeSlotAsync(timeSlotDTO);
            return CreatedAtAction(nameof(GetTimeSlotById), new { id = newTimeSlot.TimeSlotId }, newTimeSlot);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTimeSlot(int id, [FromBody] UpdateTimeSlotDTO timeSlotDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _timeSlotService.UpdateTimeSlotAsync(id, timeSlotDTO);
            if (!updated) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTimeSlot(int id)
        {
            var deleted = await _timeSlotService.DeleteTimeSlotAsync(id);
            if (!deleted) return NotFound();
            return Ok("Deleted");
        }
    }
}
