using Fillow.Models;
using Fillow.Models.partneradmin;
using Fillow.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Diagnostics;

namespace Fillow.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class JourneyOrderController : Controller
	{
		private readonly ILogger<JourneyOrderController> _logger;
		private readonly IJourneyOrderService _journeyOrderService;

		public JourneyOrderController(ILogger<JourneyOrderController> logger, IJourneyOrderService journeyOrderService)
		{
			_logger = logger;
			_journeyOrderService = journeyOrderService;
		}

		// Endpoint to create a new journey order
		[HttpPost("create")]
		public async Task<IActionResult> CreateOrder([FromBody] JourneyOrder model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
                string userId = HttpContext.Session.GetString("UserId");
				model.UserId = userId;	
                await _journeyOrderService.CreateOrderAsync(model);
				return Ok(new { message = "" });
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error creating order: {ex.Message}");
				return StatusCode(500, "Internal server error: " + ex.Message);
			}
		}

		// Endpoint to retrieve all journey orders
		[HttpGet("all")]
		public async Task<IActionResult> GetAllOrders()
		{
			try
			{
				var orders = await _journeyOrderService.GetAllOrdersAsync();
				return Ok(orders);
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error retrieving orders: {ex.Message}");
				return StatusCode(500, "Internal server error: " + ex.Message);
			}
		}

		// Endpoint to get a specific order by ID
		[HttpGet("{id}")]
		public async Task<IActionResult> GetOrderById(string id)
		{
			try
			{
				var order = await _journeyOrderService.GetOrderByIdAsync(id);
				if (order == null)
				{
					return NotFound(new { message = "Journey order not found" });
				}

				return Ok(order);
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error retrieving order by ID: {ex.Message}");
				return StatusCode(500, "Internal server error: " + ex.Message);
			}
		}

	}

}
