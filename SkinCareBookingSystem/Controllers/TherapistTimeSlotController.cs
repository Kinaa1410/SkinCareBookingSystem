using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using SkinCareBookingSystem.Enums;

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
        public async Task<ActionResult<TherapistTimeSlotDTO>> CreateTimeSlotForTherapist(int scheduleId)
        {
            // Get the list of created time slots
            var newTimeSlots = await _timeSlotService.CreateTimeSlotForTherapistAsync(scheduleId);

            // Check if the list is not empty
            if (newTimeSlots == null || !newTimeSlots.Any())
            {
                return BadRequest("No time slots were created.");
            }
            var firstTimeSlot = newTimeSlots.First();
            return CreatedAtAction(nameof(GetTimeSlotById), new { id = firstTimeSlot.Id }, firstTimeSlot);
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTimeSlot(int id, [FromBody] SlotStatus status)
        {
            var updated = await _timeSlotService.UpdateTimeSlotAsync(id, status);
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
