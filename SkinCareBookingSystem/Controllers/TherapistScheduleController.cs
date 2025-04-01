using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.DTOs;
using SkinCareBookingSystem.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TherapistScheduleController : ControllerBase
    {
        private readonly ITherapistScheduleService _therapistScheduleService;

        public TherapistScheduleController(ITherapistScheduleService therapistScheduleService)
        {
            _therapistScheduleService = therapistScheduleService;
        }

        // Get all therapist schedules
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TherapistScheduleDTO>>> GetAllSchedules()
        {
            var schedules = await _therapistScheduleService.GetAllSchedulesAsync();
            return Ok(schedules);
        }

        // Get therapist schedule by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<TherapistScheduleDTO>> GetScheduleById(int id)
        {
            var schedule = await _therapistScheduleService.GetScheduleByIdAsync(id);
            if (schedule == null)
                return NotFound();

            return Ok(schedule);
        }

        // Create a new therapist schedule
        [HttpPost]
        public async Task<ActionResult<TherapistScheduleDTO>> CreateSchedule([FromBody] CreateTherapistScheduleDTO scheduleDTO)
        {
            if (scheduleDTO.TherapistId == 0)
                return BadRequest("TherapistId is required");

            var createdSchedule = await _therapistScheduleService.CreateScheduleAsync(scheduleDTO);
            return CreatedAtAction(nameof(GetScheduleById), new { id = createdSchedule.ScheduleId }, createdSchedule);
        }

        // Update an existing therapist schedule
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSchedule(int id, [FromBody] UpdateTherapistScheduleDTO scheduleDTO)
        {
            var updated = await _therapistScheduleService.UpdateScheduleAsync(id, scheduleDTO);
            if (!updated)
                return NotFound();

            return Ok("Update success");
        }

        // Delete a therapist schedule
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            var deleted = await _therapistScheduleService.DeleteScheduleAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }

        // Get schedules by therapist ID
        [HttpGet("by-therapist/{therapistId}")]
        public async Task<ActionResult<IEnumerable<TherapistScheduleDTO>>> GetScheduleByTherapistId(int therapistId)
        {
            var schedules = await _therapistScheduleService.GetScheduleByTherapistIdAsync(therapistId);
            if (schedules == null || !schedules.Any())
                return NotFound();

            return Ok(schedules);
        }

        // Get therapists working in a specific time range
        [HttpGet("working-in-time-range")]
        public async Task<ActionResult<IEnumerable<TherapistScheduleDTO>>> GetTherapistsWorkingInTimeRange([FromQuery] string startTime, [FromQuery] string endTime)
        {
            var start = TimeSpan.Parse(startTime);
            var end = TimeSpan.Parse(endTime);
            var schedules = await _therapistScheduleService.GetTherapistsWorkingInTimeRangeAsync(start, end);
            return Ok(schedules);
        }

        // Get therapists working on a specific day and time range
        [HttpGet("working-on-day-in-time-range")]
        public async Task<ActionResult<IEnumerable<TherapistScheduleDTO>>> GetTherapistsWorkingOnDayInTimeRange([FromQuery] DayOfWeek dayOfWeek, [FromQuery] string startTime, [FromQuery] string endTime)
        {
            var start = TimeSpan.Parse(startTime);
            var end = TimeSpan.Parse(endTime);
            var schedules = await _therapistScheduleService.GetTherapistsWorkingOnDayInTimeRangeAsync(dayOfWeek, start, end);
            return Ok(schedules);
        }
    }
}
