using Fillow.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Fillow.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;

        public AdminController(ILogger<AdminController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult levelDiscount()
        {
            return View();
        }

        public IActionResult pointPage()
        {
            return View();
        }

        public IActionResult memberList()
        {
            return View();
        }

        public IActionResult bookedList()
        { 
            return View();
        }
        public IActionResult Verification()
        {
            return View();
        }

        public IActionResult bookedTour() {
            return View();
        }

        public IActionResult CategoryTour()
        {
            return View();
        }
        public IActionResult ProposalTour()
        {
            return View();
        }
        public IActionResult waitForApprovalTour()
        { 
        return View();
        }

        public IActionResult partnerPage()
        {
            return View();
        }
        public IActionResult EmailTemplate() {
            return View();
        }
        public IActionResult DirectNotice()
        {
            return View();
        }
        public IActionResult DiscountCode() {
            return View();
        }

        public IActionResult Stripe()
        {
            return View();
        }

        public IActionResult Paypal()
        {
            return View();
        }
        public IActionResult MoneyTransfer()
        {
            return View();
        }
        public IActionResult Alipay()
        {
            return View();
        }

        public IActionResult Wechat()
        {
            return View();
        }
        public IActionResult AutoNotice()
        {
            return View();
        }
        public IActionResult PaymentFromPartner()
        {
            return View();
        }
        public IActionResult CreateTrip() {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}