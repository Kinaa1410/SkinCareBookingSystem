using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.Interfaces;
using SkinCareBookingSystem.DTOs;
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

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<TherapistScheduleDTO>>> GetAllSchedules()
        {
            var schedules = await _therapistScheduleService.GetAllSchedulesAsync();
            return Ok(schedules);
        }

        [HttpGet("{scheduleId}")]
        public async Task<ActionResult<TherapistScheduleDTO>> GetScheduleById(int scheduleId)
        {
            var schedule = await _therapistScheduleService.GetScheduleByIdAsync(scheduleId);
            if (schedule == null)
                return NotFound();

            return Ok(schedule);
        }

        [HttpPost]
        public async Task<ActionResult<TherapistScheduleDTO>> CreateSchedule([FromBody] CreateTherapistScheduleDTO scheduleDTO)
        {
            var createdSchedule = await _therapistScheduleService.CreateScheduleAsync(scheduleDTO);
            return CreatedAtAction(nameof(GetScheduleById), new { scheduleId = createdSchedule.ScheduleId }, createdSchedule);
        }

        [HttpPut("{scheduleId}")]
        public async Task<ActionResult> UpdateSchedule(int scheduleId, [FromBody] UpdateTherapistScheduleDTO scheduleDTO)
        {
            var result = await _therapistScheduleService.UpdateScheduleAsync(scheduleId, scheduleDTO);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{scheduleId}")]
        public async Task<ActionResult> DeleteSchedule(int scheduleId)
        {
            var result = await _therapistScheduleService.DeleteScheduleAsync(scheduleId);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("working-on-day/{dayOfWeek}")]
        public async Task<ActionResult<IEnumerable<TherapistScheduleDTO>>> GetTherapistsWorkingOnDay(DayOfWeek dayOfWeek)
        {
            var schedules = await _therapistScheduleService.GetTherapistsWorkingOnDayAsync(dayOfWeek);
            return Ok(schedules);
        }

        [HttpGet("working-in-time-range")]
        public async Task<ActionResult<IEnumerable<TherapistScheduleDTO>>> GetTherapistsWorkingInTimeRange([FromQuery] string startTime, [FromQuery] string endTime)
        {
            var start = TimeSpan.Parse(startTime);
            var end = TimeSpan.Parse(endTime);
            var schedules = await _therapistScheduleService.GetTherapistsWorkingInTimeRangeAsync(start, end);
            return Ok(schedules);
        }

        [HttpGet("working-on-day-in-time-range")]
        public async Task<ActionResult<IEnumerable<TherapistScheduleDTO>>> GetTherapistsWorkingOnDayInTimeRange(DayOfWeek dayOfWeek, [FromQuery] string startTime, [FromQuery] string endTime)
        {
            var start = TimeSpan.Parse(startTime);
            var end = TimeSpan.Parse(endTime);
            var schedules = await _therapistScheduleService.GetTherapistsWorkingOnDayInTimeRangeAsync(dayOfWeek, start, end);
            return Ok(schedules);
        }

        [HttpGet("therapist/{therapistId}")]
        public async Task<ActionResult<IEnumerable<TherapistScheduleDTO>>> GetScheduleByTherapistId(int therapistId)
        {
            var schedules = await _therapistScheduleService.GetScheduleByTherapistIdAsync(therapistId);
            if (schedules == null || !schedules.Any())
                return NotFound();

            return Ok(schedules);
        }
    }
}
