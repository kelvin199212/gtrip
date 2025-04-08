using Fillow.Models;
using Fillow.Models.partneradmin;
using Fillow.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Fillow.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserProfileService _userProfileService;
        private readonly ITravelPlanService _travelPlanService;
        private readonly IWithdrawalService _withdrawalService;


        public HomeController(IUserProfileService userProfileService, ITravelPlanService travelPlanService, IWithdrawalService withdrawalService, ILogger<HomeController> logger)
        {
            _logger = logger;
            _userProfileService = userProfileService;
            _travelPlanService = travelPlanService;
            _withdrawalService = withdrawalService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult BecomeOurPartner() {
            return View();
        }

        public IActionResult Quotation()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }
        public IActionResult ContactUs() {
            return View();
        }

        public IActionResult HotTrip() {
            return View();
        }

        public IActionResult HotTripbackup()
        {
            return View();
        }

        public IActionResult Tour() {
            return View();
        }

        public IActionResult Membership() {
            return View();
        }

        public IActionResult AddBlog()
        {
            return View();
        }

        public IActionResult Content()
        {
            return View();
        }

        public IActionResult CreateOrder()
        {

            return View();
        }

        public IActionResult CreateTrip()
        {
            var travelTypes = _travelPlanService.GetActiveTravelTypes();
            return View(travelTypes);
        }

        public async Task<IActionResult> EditProfile()
        {
            // Retrieve the UserId from the session
            var userId = HttpContext.Session.GetString("UserId");

            // Check if UserId is null or empty
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Home"); // Redirect to login if not authenticated
            }

            // Retrieve the user profile from the database
            var userProfile = await _userProfileService.GetUserProfileByUserIdAsync(userId);

            if (userProfile == null)
            {
                userProfile = new UserProfile();
            }

            // Pass the user profile to the view
            return View(userProfile);
        }

        public async Task<IActionResult> EditTrip()
        {
            var userId = HttpContext.Session.GetString("UserId");
            List<TripViewModel> trips = await _travelPlanService.GetTripsForUserAsync(userId);

            return View(trips);
        }

        [HttpGet]
        public IActionResult EditTripDetail(string tripId) {


            var tripViewModel = _travelPlanService.GetTripViewModel(tripId);
            var travelTypes = _travelPlanService.GetActiveTravelTypes();
            var combinedViewModel = new CombinedTripViewModel
            {
                TripViewModel = tripViewModel,
                TravelTypes = travelTypes
            };

            return View(combinedViewModel);



        }

        public IActionResult WithdrawalRequest() {
            var userId = HttpContext.Session.GetString("UserId");
            List<WithdrawalRequestModel> list = new List<WithdrawalRequestModel>();
            list = _withdrawalService.GetWithdrawalRecords(userId);
            return View(list);

        }
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult ProductDetails()
        {
            return View();
        }

        public IActionResult ProductList()
        {
            return View();
        }

        public IActionResult Verification()
        {
            var Usertype = HttpContext.Session.GetString("UserType");
            if (Usertype == "normal")
            {
                return View();
            }
            else {
                return NotFound();
            }
        }

        public IActionResult VerificationB()
        {
            var Usertype = HttpContext.Session.GetString("UserType");
            if (Usertype != "normal")
            {
                return View();
            }
            else
            {
                return NotFound();
            }
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}