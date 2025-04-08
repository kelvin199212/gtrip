using Fillow.Models;
using Fillow.Models.partneradmin;
using Fillow.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fillow.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TravelController : Controller
    {
        private readonly ITravelPlanService _travelPlanService;
        private readonly ILogger<TravelController> _logger;

        public TravelController(ITravelPlanService travelPlanService, ILogger<TravelController> logger)
        {
            _travelPlanService = travelPlanService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] IFormCollection formData)
        {

            var userId = HttpContext.Session.GetString("UserId");
            await _travelPlanService.InsertTripFormDataAsync(formData, userId);

        
            return Ok(new { message = "Trip Create successfully" });
        }

        [HttpPost("EditTrip")]
        public async Task<IActionResult> EditTrip([FromForm] IFormCollection formData)
        {

            var userId = HttpContext.Session.GetString("UserId");
            await _travelPlanService.UpdateTripFormDataAsync(formData, userId);


            return Ok(new { message = "Trip Edit successfully" });
        }


        [HttpGet("{tripId}")]
        public ActionResult<TripViewModel> GetTripViewModel(string tripId)
        {
            try
            {
                var tripViewModel = _travelPlanService.GetTripViewModel(tripId);

                TempData["TripViewModel"] = JsonConvert.SerializeObject(tripViewModel);

                return Ok(tripViewModel);

            }
            catch (Exception ex)
            {
                // Log the error and return a 404 Not Found or 500 Internal Server Error
                return NotFound(new { message = ex.Message }); // Handle error accordingly
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllTravelPlans()
        {
            try
            {
                var travelPlans = await _travelPlanService.GetTravelPlansAsync();
                return Ok(travelPlans);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching travel plans");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetTravelPlanById(string id)
        //{
        //    try
        //    {

        //        var travelPlan = await _travelPlanService.GetTravelPlanByIdAsync(id);
        //        if (travelPlan == null)
        //        {
        //            return NotFound(new { message = "Travel plan not found" });
        //        }
        //        return Ok(travelPlan);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error fetching travel plan");
        //        return StatusCode(500, "Internal server error: " + ex.Message);
        //    }
        //}

        [HttpPost("add")]
        public async Task<IActionResult> AddTravelPlan([FromBody] TravelPlan newPlan)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _travelPlanService.CreateTravelPlanAsync(newPlan);
                return Ok(new { message = "Travel plan added successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding travel plan");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateTravelPlan(string id, [FromBody] TravelPlan updatedPlan)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {

                var existingPlan = await _travelPlanService.GetTravelPlanByIdAsync(id);
                if (existingPlan == null)
                {
                    return NotFound(new { message = "Travel plan not found" });
                }

                await _travelPlanService.UpdateTravelPlanAsync(id, updatedPlan);
                return Ok(new { message = "Travel plan updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating travel plan");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteTravelPlan(string id)
        {
            try
            {

                var existingPlan = await _travelPlanService.GetTravelPlanByIdAsync(id);
                if (existingPlan == null)
                {
                    return NotFound(new { message = "Travel plan not found" });
                }

                await _travelPlanService.DeleteTravelPlanAsync(id);
                return Ok(new { message = "Travel plan deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting travel plan");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}
