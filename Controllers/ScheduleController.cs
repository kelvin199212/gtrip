using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Fillow.Models.partneradmin;
using Fillow.Services;
using MongoDB.Bson;

namespace Fillow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : Controller
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Schedule>>> GetSchedules()
        {
            var schedules = await _scheduleService.GetSchedulesAsync();
            return Ok(schedules);
        }

        [HttpPost("create")]
        public async Task<ActionResult<Schedule>> CreateSchedule([FromBody] Schedule schedule)
        {
            string userId = HttpContext.Session.GetString("UserId");
            schedule.userId = userId;
            var createdSchedule = await _scheduleService.CreateScheduleAsync(schedule);
            return CreatedAtAction(nameof(GetSchedules), new { id = createdSchedule.Id }, createdSchedule);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Schedule>> UpdateSchedule(string id, [FromBody] Schedule updatedSchedule)
        {


            try
            {
                var updated = await _scheduleService.UpdateScheduleAsync(id, updatedSchedule);
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSchedule(string id)
        {
      
            try
            {
                await _scheduleService.DeleteScheduleAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }

}
