using Fillow.Models;
using Fillow.Models.partneradmin;
using Fillow.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Fillow.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WithdrawalController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<HomeController> _logger;
        private readonly IWithdrawalService _withdrawalService;
        public WithdrawalController(IConfiguration configuration, ILogger<HomeController> logger, IWithdrawalService userService)
        {
            _logger = logger;
            _withdrawalService = userService;
            _configuration = configuration;
        }


        [HttpPost("create")]
        public IActionResult SubmitWithdrawal(WithdrawalRequestModel model)
        {
            if (ModelState.IsValid)
            {
                model.UserId = HttpContext.Session.GetString("UserId");
                model.createDate = DateTime.Today;
                model.Status = "批核中";
                var result = _withdrawalService.ProcessWithdrawalRequest(model);
                if (result)
                {
                    return Ok(new { message = "成功提交!" });
                }
                else
                {
                    return NotFound(new { message = "提交失敗!" });
                }

            }
            return View("Index", model); // Return to the same view if validation fails
        }

    }
}