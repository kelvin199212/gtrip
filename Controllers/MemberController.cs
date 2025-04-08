using Fillow.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Fillow.Controllers
{
    public class MemberController : Controller
    {
        private readonly ILogger<MemberController> _logger;

        public MemberController(ILogger<MemberController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }
        public IActionResult MyOrder()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult Payment()
        {
            return View();
        }

        public IActionResult Preference()
        {
            return View();
        }

        public IActionResult Coupon() {
            return View();
        }

        public IActionResult ReportCenter() {
            return View();
        }

        public IActionResult PwdChange() {
            return View();
        }

        public IActionResult LoginMethod()
        {
            return View();
        }

        public IActionResult NoticePreference()
        {
            return View();
        }

        public IActionResult Verification()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}